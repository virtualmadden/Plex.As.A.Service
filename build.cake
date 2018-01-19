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
var projDir = Directory("./Plex.As.A.Service/Plex.Service/bin/debug/net461");
var testDir = Directory("./Plex.As.A.Service/Plex.Service.Common.Tests/bin/debug/net461");
var solution = "./Plex.As.A.Service.sln";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Description("Cleaning the build directory.")
    .Does(() =>
{
    CleanDirectory(projDir);
    CleanDirectory(testDir);
});

Task("Restore")
    .Description("Restoring NuGet packages.")
    .Does(() =>
{
    DotNetCoreRestore(solution);
});

Task("Build")
    .Description("Building the solution.")
    .Does(() =>
{
    DotNetCoreBuild(solution); //, buildSettings);
});

Task("Test")
    .Description("Runs Unit tests.")
    .Does(() =>
{
    var unitTests = GetFiles("./Plex.As.A.Service/Plex.Service.Common.Tests/*.csproj");

    foreach(var test in unitTests)
    {
        DotNetCoreTest(test.FullPath);
    }
});

Task("Install")
    .Description("Installs the service.")
    .Does(() =>
{
    InstallTopshelf($"{projDir}/Plex.Service.exe", new TopshelfSettings()
    {
        Autostart = true,
        LocalSystem = true,
        Delayed = true
    });
});

Task("Start")
    .Description("Starts the Service.")
    .Does(() =>
{
    StartTopshelf("" + projDir + "/Plex.Service.exe");
});

Task("Stop")
    .Description("Stops the service.")
    .Does(() =>
{
    StopTopshelf("" + projDir + "/Plex.Service.exe");
});

Task("Uninstall")
    .Description("Uninstalls the service.")
    .Does(() =>
{
    UninstallTopshelf("" + projDir + "/Plex.Service.exe");
});

Task("")

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Uninstall");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
