var target          = Argument("target", "Default");
var configuration   = Argument<string>("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////
var buildArtifacts      = Directory("./artifacts/packages");
var packageVersion      = "2.2.1";

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
});


Task("Default")
  .IsDependentOn("Pack");

RunTarget(target);