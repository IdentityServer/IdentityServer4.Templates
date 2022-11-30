# IdentityServer4.Templates
dotnet new templates for IdentityServer4

## Important
This organization is not maintained anymore besides critical security bugfixes (if feasible). This organization will be archived when .NET Core 3.1 end of support is reached (3rd Dec 2022). All new development is happening in the new [Duende Software](https://github.com/duendesoftware) organization. 

The new [Duende IdentityServer](https://duendesoftware.com/products/identityserver) comes with a commercial license but is [free](https://blog.duendesoftware.com/posts/20220111_fair_trade/) for dev/testing/personal projects and companies or individuals making less than 1M USD gross annnual revenue. Please [get in touch with us](https://duendesoftware.com/contact) if you have any question.

### dotnet new is4empty
Creates a minimal IdentityServer4 project without a UI.

### dotnet new is4ui
Adds the quickstart UI to the current project (can be e.g added on top of *is4empty*)

### dotnet new is4inmem
Adds a basic IdentityServer with UI, test users and sample clients and resources. Shows both in-memory code and JSON configuration.

### dotnet new is4aspid
Adds a basic IdentityServer that uses ASP.NET Identity for user management. If you automatically seed the database, you will get two users: `alice` and `bob` - both with password `Pass123$`. Check the `SeedData.cs` file.

### dotnet new is4ef
Adds a basic IdentityServer that uses Entity Framework for configuration and state management. If you seed the database, you get a couple of basic client and resource registrations, check the `SeedData.cs` file.

### dotnet new is4admin
Adds an IdentityServer that includes the Rock Solid Knowledge AdminUI Community Edition (open `http://localhost:5000/admin` in the browser). This gives you a web-based administration interface for users, claims, clients and resources.

The community edition is intended for testing IdentityServer integration scenarios and is limited to localhost:5000, SQLite, 10 users, and 2 clients. *The community edition is not suitable for production*.

See [identityserver.com](https://www.identityserver.com/products) for more information about AdminUI or to request a trial license.

## Installation 

Install with:

`dotnet new -i identityserver4.templates`

If you need to set back your dotnet new list to "factory defaults", use this command:

`dotnet new --debug:reinit`
