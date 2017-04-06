var target          = Argument("target", "Default");
var configuration   = Argument<string>("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////
var buildArtifacts      = Directory("./artifacts/packages");
var packageVersion      = "1.0.0";

///////////////////////////////////////////////////////////////////////////////
// Clean
///////////////////////////////////////////////////////////////////////////////
Task("Clean")
    .Does(() =>
{
    CleanDirectories(new DirectoryPath[] { buildArtifacts });
});

///////////////////////////////////////////////////////////////////////////////
// Copy
///////////////////////////////////////////////////////////////////////////////
Task("Copy")
    .IsDependentOn("Clean")
    .Does(() =>
{
    CreateDirectory("./feed/content");

    var files = GetFiles("./src/**/*.*");
    CopyFiles(files, "./feed/content", true);
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