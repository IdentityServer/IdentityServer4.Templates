# IdentityServer4.Templates
dotnet new templates for IdentityServer4

### dotnet new is4empty
Creates a minimal IdentityServer4 project without a UI.

### dotnet new is4ui
Adds the quickstart UI to the current project (can be e.g added on top of *is4empty*)

### dotnet new is4inmem
Adds a basic IdentityServer with UI, test users and sample clients and resources. Shows both in-memory code and JSON configuration.

### dotnet new is4aspid
Adds a basic IdentityServer that uses ASP.NET Identity for user management. If you automatically seed the database, you will get two users: alice and bob. Both with password `Pass123$`. Check the `SeedData.cs` file.

### dotnet new is4ef
Adds a basic IdentityServer that uses Entity Framework for configuration and state management. If you seed the database you get a couple of basic client and resource registrations, check the `SeedData.cs` file.

## Installation 

Install with:

`dotnet new -i identityserver4.templates`

If you need to set back your dotnet new list to "factory defaults", use this command:

`dotnet new --debug:reinit`
