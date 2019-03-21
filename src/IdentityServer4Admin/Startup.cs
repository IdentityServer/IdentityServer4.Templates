// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4Admin.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using IdentityExpress.Identity;
using IdentityExpress.Manager.Api;
using IdentityServer4.Configuration;
using IdentityServer4.Services;
using IdentityServer4Admin.Data;
using Microsoft.Extensions.Logging.Abstractions;
using RSK.Audit.EF;
using RSK.IdentityServer4.AuditEventSink;

namespace IdentityServer4Admin
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("Users")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            var connectionString = Configuration.GetConnectionString("Configuration");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                options.UserInteraction = new UserInteractionOptions
                {
                    LogoutUrl = "/Account/Logout",
                    LoginUrl = "/Account/Login",
                    LoginReturnUrlParameter = "returnUrl"
                };
            })
                .AddAspNetIdentity<ApplicationUser>()
                // this adds the config data from DB (clients, resources, CORS)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = db =>
                        db.UseSqlite(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = db =>
                        db.UseSqlite(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    // options.TokenCleanupInterval = 15; // interval in seconds. 15 seconds useful for debugging
                });

            ConfigureIdentityServerAuditing(services, connectionString);

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                throw new Exception("need to configure key material");
            }

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    // register your IdentityServer with Google at https://console.developers.google.com
                    // enable the Google+ API
                    // set the redirect URI to http://localhost:5000/signin-google
                    options.ClientId = "copy client ID from Google here";
                    options.ClientSecret = "copy client secret from Google here";
                });

            services.UseAdminUI();
            services.AddScoped<IdentityExpressDbContext, SqliteIdentityDbContext>();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIdentityServer();

            app.UseCommunityLogin();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
            app.UseAdminUI();
        }
        
        public void ConfigureIdentityServerAuditing(IServiceCollection services, string auditConnectionString)
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<AuditDatabaseContext>();
            RSK.Audit.AuditProviderFactory auditFactory = new AuditProviderFactory(dbContextOptionsBuilder.UseSqlite(auditConnectionString).Options);
            var auditRecorder = auditFactory.CreateAuditSource("IdentityServer");
            services.AddSingleton<IEventSink>(provider => new AuditSink(auditRecorder));

            services.AddSingleton<IEventSink>(provider => new EventSinkAggregator(new NullLogger<EventSinkAggregator>())
            {
                EventSinks = new List<IEventSink>
                {
                    new AuditSink(auditRecorder)
                }
            });
        }
    }
}
