var target          = Argument("target", "Default");
var configuration   = Argument<string>("configuration", "Release");
var sign            = Argument<bool>("sign", false);

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////
var buildArtifacts      = Directory("./artifacts");
var packageVersion      = "2.6.1";

///////////////////////////////////////////////////////////////////////////////
// Clean
///////////////////////////////////////////////////////////////////////////////
Task("Clean")
    .Does(() =>
{
    CleanDirectories(new DirectoryPath[] 
    {
        buildArtifacts,
        Directory("./feed/content/UI")
    });
});

///////////////////////////////////////////////////////////////////////////////
// Build
///////////////////////////////////////////////////////////////////////////////
Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
{
    var settings = new DotNetCoreBuildSettings 
    {
        Configuration = configuration,
    };

    var projects = GetFiles("./src/**/*.csproj");
    foreach(var project in projects)
	{
	    DotNetCoreBuild(project.GetDirectory().FullPath, settings);
    }
});

///////////////////////////////////////////////////////////////////////////////
// Copy
///////////////////////////////////////////////////////////////////////////////
Task("Copy")
    .IsDependentOn("Clean")
    .Does(() =>
{
    CreateDirectory("./feed/content");

    // copy the singel csproj templates
    var files = GetFiles("./src/**/*.*");
    CopyFiles(files, "./feed/content", true);

    // copy the UI files
    files = GetFiles("./ui/**/*.*");
    CopyFiles(files, "./feed/content/ui", true);
});

///////////////////////////////////////////////////////////////////////////////
// Pack
///////////////////////////////////////////////////////////////////////////////
Task("Pack")
    .IsDependentOn("Clean")
    .IsDependentOn("Copy")
    .Does(() =>
{
    var settings = new NuGetPackSettings
    {
        Version = packageVersion,
        OutputDirectory = buildArtifacts
    };

    if (AppVeyor.IsRunningOnAppVeyor)
    {
        settings.Version = packageVersion + "-b" + AppVeyor.Environment.Build.Number.ToString().PadLeft(4,'0');
    }

    NuGetPack("./feed/IdentityServer4.Templates.nuspec", settings);
    
    if (sign)
    {
        var packages = GetFiles("./artifacts/**/*.nupkg");
        foreach(var package in packages)
        {
            SignFile(package.FullPath);
        }
    }
});



private void SignFile(string fileName)
{
    var signClientConfig = EnvironmentVariable("SignClientConfig") ?? "";
    var signClientSecret = EnvironmentVariable("SignClientSecret") ?? "";

    if (signClientConfig == "")
    {
        throw new Exception("SignClientConfig environment variable is missing. Aborting.");
    }

    if (signClientSecret == "")
    {
        throw new Exception("SignClientSecret environment variable is missing. Aborting.");
    }

    Information("  Signing " + fileName);

    var success = StartProcess("./tools/signclient", new ProcessSettings {
        Arguments = new ProcessArgumentBuilder()
            .Append("sign")
            .Append($"-c {signClientConfig}")
            .Append($"-i {fileName}")
            .Append("-r sc-ids@dotnetfoundation.org")
            .Append($"-s {signClientSecret}")
            .Append(@"-n 'IdentityServer4'")
        });

    if (success == 0)
    {
        Information("  success.");
    }
    else
    {
        throw new Exception("Error signing " + fileName);
    }
    
}


Task("Default")
  .IsDependentOn("Pack");

RunTarget(target);