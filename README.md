# IdentityServer4.Templates
dotnet new templates for IdentityServer4

### dotnet new is4
Creates a minimal IdentityServer4 project without a UI and just one API and one client.

### dotnet new is4ui
Adds the quickstart UI to the current project (can be combined with *is4*)

### dotnet new is4inmem
Adds a boilerplate IdentityServer with UI, test users and sample clients and resources

### dotnet new is4aspid
Adds a basic IdentityServer that uses ASP.NET Identity for user management

### dotnet new is4ef
Adds a basic IdentityServer that uses Entity Framework for configuration and state management

## Installation 

Install with:

`dotnet new -i identityserver4.templates`

If you need to set back your dotnet new list to "factory defaults", use this command:

`dotnet new --debug:reinit`
