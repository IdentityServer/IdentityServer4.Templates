// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using IdentityServer4.Events;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using IdentityServer4AspNetIdentity.Models;
using IdentityServer4.Extensions;
using System.Security.Principal;
using System.Security.Claims;
using IdentityModel;
using System.Linq;
using System;
using System.Collections.Generic;

namespace IdentityServer4.Quickstart.UI
{
    [SecurityHeaders]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;
        private readonly AccountService _account;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IHttpContextAccessor httpContextAccessor,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _events = events;
            _account = new AccountService(interaction, httpContextAccessor, schemeProvider, clientStore);
        }

        /// <summary>
        /// Show login page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            // build a model so we know what to show on the login page
            var vm = await _account.BuildLoginViewModelAsync(returnUrl);

            if (vm.IsExternalLoginOnly)
            {
                // we only have one option for logging in and it's an external provider
                return await ExternalLogin(vm.ExternalLoginScheme, returnUrl);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model, string button)
        {
            if (button != "login")
            {
                // the user clicked the "cancel" button
                var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
                if (context != null)
                {
                    // if the user cancels, send a result back into IdentityServer as if they 
                    // denied the consent (even if this client does not require consent).
                    // this will send back an access denied OIDC error response to the client.
                    await _interaction.GrantConsentAsync(context, ConsentResponse.Denied);
                    
                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    // since we don't have a valid context, then we just go back to the home page
                    return Redirect("~/");
                }
            }

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberLogin, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.Username);
                    await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName));

                    // make sure the returnUrl is still valid, and if so redirect back to authorize endpoint or a local page
                    if (_interaction.IsValidReturnUrl(model.ReturnUrl) || Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return Redirect("~/");
                }

                await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials"));

                ModelState.AddModelError("", AccountOptions.InvalidCredentialsErrorMessage);
            }

            // something went wrong, show form with error
            var vm = await _account.BuildLoginViewModelAsync(model);
            return View(vm);
        }

        /// <summary>
        /// initiate roundtrip to external authentication provider
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExternalLogin(string provider, string returnUrl)
        {
            var props = new AuthenticationProperties()
            {
                RedirectUri = Url.Action("ExternalLoginCallback"),
                Items =
                {
                    { "returnUrl", returnUrl }
                }
            };

            // windows authentication needs special handling
            // since they don't support the redirect uri, 
            // so this URL is re-triggered when we call challenge
            if (AccountOptions.WindowsAuthenticationSchemeName == provider)
            {
                // see if windows auth has already been requested and succeeded
                var result = await HttpContext.AuthenticateAsync(AccountOptions.WindowsAuthenticationSchemeName);
                if (result?.Principal is WindowsPrincipal wp)
                {
                    props.Items.Add("scheme", AccountOptions.WindowsAuthenticationSchemeName);

                    var id = new ClaimsIdentity(provider);
                    id.AddClaim(new Claim(JwtClaimTypes.Subject, wp.Identity.Name));
                    id.AddClaim(new Claim(JwtClaimTypes.Name, wp.Identity.Name));

                    // add the groups as claims -- be careful if the number of groups is too large
                    if (AccountOptions.IncludeWindowsGroups)
                    {
                        var wi = wp.Identity as WindowsIdentity;
                        var groups = wi.Groups.Translate(typeof(NTAccount));
                        var roles = groups.Select(x => new Claim(JwtClaimTypes.Role, x.Value));
                        id.AddClaims(roles);
                    }

                    await HttpContext.SignInAsync(
                        IdentityServer4.IdentityServerConstants.ExternalCookieAuthenticationScheme,
                        new ClaimsPrincipal(id),
                        props);
                    return Redirect(props.RedirectUri);
                }
                else
                {
                    // challenge/trigger windows auth
                    return Challenge(AccountOptions.WindowsAuthenticationSchemeName);
                }
            }
            else
            {
                // start challenge and roundtrip the return URL
                props.Items.Add("scheme", provider);
                return Challenge(props, provider);
            }
        }

        /// <summary>
        /// Post processing of external authentication
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            // read external identity from the temporary cookie
            var result = await HttpContext.AuthenticateAsync(IdentityServer4.IdentityServerConstants.ExternalCookieAuthenticationScheme);
            if (result?.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }

            // retrieve claims of the external user
            var externalUser = result.Principal;
            var claims = externalUser.Claims.ToList();

            // try to determine the unique id of the external user (issued by the provider)
            // the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used
            var userIdClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject);
            if (userIdClaim == null)
            {
                userIdClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            }
            if (userIdClaim == null)
            {
                throw new Exception("Unknown userid");
            }

            // remove the user id claim from the claims collection and move to the userId property
            // also set the name of the external authentication provider
            claims.Remove(userIdClaim);
            var provider = result.Properties.Items["scheme"];
            var userId = userIdClaim.Value;

            // this is where custom logic would most likely be needed to match your users from the
            // external provider's authentication result, and provision the user as you see fit.
            // 
            // check if the external user is already provisioned
            var user = await _userManager.FindByLoginAsync(provider, userId);
            if (user == null)
            {
                // this sample simply auto-provisions new external user
                // another common approach is to start a registrations workflow first
                // a common way to build a registration page is to use the claims from the 
                // external provider to pre-populate a registration screen, and then store those
                // as claims (or custom properties) in the ASP.NET Identity database.
                user = new ApplicationUser
                {
                    UserName = Guid.NewGuid().ToString(),
                };
                var identityResult = await _userManager.CreateAsync(user);
                if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);

                identityResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, userId, provider));
                if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);
            }

            var additionalClaims = new List<Claim>();

            // if the external system sent a session id claim, copy it over
            // so we can use it for single sign-out
            var sid = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                additionalClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            // if the external provider issued an id_token, we'll keep it for signout
            AuthenticationProperties props = null;
            var id_token = result.Properties.GetTokenValue("id_token");
            if (id_token != null)
            {
                props = new AuthenticationProperties();
                props.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = id_token } });
            }

            var name = externalUser.FindFirst(JwtClaimTypes.Name)?.Value;
            if (name == null) name = externalUser.FindFirst(ClaimTypes.Name)?.Value;
            if (name == null) name = user.Id;

            // issue authentication cookie for user
            await _events.RaiseAsync(new UserLoginSuccessEvent(provider, userId, user.Id, name));
            await HttpContext.SignInAsync(user.Id, name, provider, props, additionalClaims.ToArray());

            // delete temporary cookie used during external authentication
            await HttpContext.SignOutAsync(IdentityServer4.IdentityServerConstants.ExternalCookieAuthenticationScheme);

            // validate return URL and redirect back to authorization endpoint or a local page
            var returnUrl = result.Properties.Items["returnUrl"];
            if (_interaction.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("~/");
        }

        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            // build a model so the logout page knows what to display
            var vm = await _account.BuildLogoutViewModelAsync(logoutId);

            if (vm.ShowLogoutPrompt == false)
            {
                // if the request for logout was properly authenticated from IdentityServer, then
                // we don't need to show the prompt and can just log the user out directly.
                return await Logout(vm);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            // build a model so the logged out page knows what to display
            var vm = await _account.BuildLoggedOutViewModelAsync(model.LogoutId);

            var user = HttpContext.User;
            if (user?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await _signInManager.SignOutAsync();

                // raise the logout event
                await _events.RaiseAsync(new UserLogoutSuccessEvent(user.GetSubjectId(), user.GetDisplayName()));
            }

            // check if we need to trigger sign-out at an upstream identity provider
            if (vm.TriggerExternalSignout)
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
            }

            return View("LoggedOut", vm);
        }
    }
}