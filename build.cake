var target          = Argument("target", "Default");
var configuration   = Argument<string>("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////
var buildArtifacts      = Directory("./artifacts/packages");
var packageVersion      = "2.5.0";

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
    .IsDependentOn("Build")
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
});


Task("Default")
  .IsDependentOn("Pack");

RunTarget(target);