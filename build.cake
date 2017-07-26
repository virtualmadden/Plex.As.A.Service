#addin "Cake.Topshelf"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./Plex.As.A.Service/bin") + Directory(configuration);
var solution = "./Plex.As.A.Service/Plex.As.A.Service.sln";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Description("Cleaning the build directory.")
    .Does(() =>
{
    CleanDirectory(buildDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Description("Restoring NuGet packages.")
    .Does(() =>
{
    NuGetRestore(solution);
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Description("Building the solution.")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
      // Use MSBuild
      MSBuild(solution, settings => settings.SetConfiguration(configuration));
    }
    else
    {
      // Use XBuild
      XBuild(solution, settings => settings.SetConfiguration(configuration));
    }
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Description("Runs Unit tests.")
    .Does(() =>
{
    NUnit3("./src/**/bin/" + configuration + "/*.Tests.dll", new NUnit3Settings {
        NoResults = true
        });
});

Task("Install")
    .IsDependentOn("Run-Unit-Tests")
    .Description("Installs the service.")
    .Does(() =>
{
    InstallTopshelf("" + buildDir + "/Plex.As.A.Service.exe", new TopshelfSettings()
    {
        Autostart = true,
        LocalSystem = true,
        Delayed = true
    });
});

Task("Start")
    .IsDependentOn("Install")
    .Description("Starts the Service.")
    .Does(() =>
{
    StartTopshelf("" + buildDir + "/Plex.As.A.Service.exe");
});

Task("Stop")
    .IsDependentOn("Start")
    .Description("Stops the service.")
    .Does(() =>
{
    StopTopshelf("" + buildDir + "/Plex.As.A.Service.exe");
});

Task("Uninstall")
    .IsDependentOn("Stop")
    .Description("Uninstalls the service.")
    .Does(() =>
{
    UninstallTopshelf("" + buildDir + "/Plex.As.A.Service.exe");
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Uninstall");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
