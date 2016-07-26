#tool "GitVersion.CommandLine"
#tool "xunit.runner.console"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target                  = Argument("target", "Default");
var configuration           = Argument("configuration", "Release");
var solutionPath            = MakeAbsolute(File(Argument("solutionPath", "./Storm.GoogleAnalytics.sln")));

var testProjects            = Enumerable.Empty<string>();

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var testAssemblyBinFormat   = "./tests/{0}/bin/" +configuration +"/{0}.dll";

var artifacts               = MakeAbsolute(Directory(Argument("artifactPath", "./artifacts")));
var buildOutput             = MakeAbsolute(Directory(artifacts +"/build/"));
var versionAssemblyInfo     = MakeAbsolute(File(Argument("versionAssemblyInfo", "VersionAssemblyInfo.cs")));

SolutionParserResult solution               = null;
GitVersion versionInfo                      = null;

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Setup(() => {
    if(!FileExists(solutionPath)) throw new Exception(string.Format("Solution file not found - {0}", solutionPath.ToString()));
    solution = ParseSolution(solutionPath.ToString());
    Information("[Setup] Using Solution '{0}'", solutionPath.ToString());
});

Task("Clean")
    .Does(() =>
{
    CleanDirectories(artifacts.ToString());
    CreateDirectory(artifacts);
    CreateDirectory(buildOutput);
    
    var binDirs = GetDirectories(solutionPath.GetDirectory() +@"\src\**\bin");
    var objDirs = GetDirectories(solutionPath.GetDirectory() +@"\src\**\obj");
    CleanDirectories(binDirs);
    CleanDirectories(objDirs);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore(solutionPath, new NuGetRestoreSettings());
});

Task("Update-Version-Info")
    .IsDependentOn("CreateVersionAssemblyInfo")
    .Does(() => 
{
        versionInfo = GitVersion(new GitVersionSettings {
            UpdateAssemblyInfo = true,
            UpdateAssemblyInfoFilePath = versionAssemblyInfo
        });

    if(versionInfo != null) {
        Information("Version: {0}", versionInfo.FullSemVer);
    } else {
        throw new Exception("Unable to determine version");
    }
});

Task("CreateVersionAssemblyInfo")
    .WithCriteria(() => !FileExists(versionAssemblyInfo))
    .Does(() =>
{
    Information("Creating version assembly info");
    CreateAssemblyInfo(versionAssemblyInfo, new AssemblyInfoSettings {
        Version = "0.0.0.0",
        FileVersion = "0.0.0.0",
        InformationalVersion = "",
    });
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .IsDependentOn("Update-Version-Info")
    .Does(() =>
{
    MSBuild(solutionPath, settings => settings
        .WithProperty("TreatWarningsAsErrors","true")
        .WithProperty("UseSharedCompilation", "false")
        .WithProperty("AutoParameterizationWebConfigConnectionStrings", "false")
        .SetVerbosity(Verbosity.Quiet)
        .SetConfiguration(configuration)
        .WithTarget("Clean;Build")
    );
});

Task("Copy-Files")
    .IsDependentOn("Build")
    .Does(() => 
{
    foreach(var project in solution.Projects) {
        var projectName = project.Name;
        var projectDir = project.Path.GetDirectory();
        var projectBuildDir = buildOutput +"/" +projectName +"/lib/net45";
        EnsureDirectoryExists(projectBuildDir);
        CopyFiles(projectDir +"/bin/" +configuration +"/" +projectName +".dll", projectBuildDir);
        CopyFiles(projectDir +"/bin/" +configuration +"/" +projectName +".xml", projectBuildDir);
        CopyFiles(projectDir +"/bin/" +configuration +"/" +projectName +".pdb", projectBuildDir);
    }
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .WithCriteria(() => testProjects.Any())
    .Does(() => 
{
    var testResultsPath = MakeAbsolute(Directory(artifacts + "/test-results"));

    XUnit2(testProjects.Select(x => string.Format(testAssemblyBinFormat, x)), new XUnit2Settings() {
        OutputDirectory = testResultsPath
    });
});

Task("Create-NuGet-Packages")
    .IsDependentOn("Build")
    .IsDependentOn("Copy-Files")
    .Does(() => 
{
    var outputDirectory = artifacts +"/packages";
    EnsureDirectoryExists(outputDirectory);

    var settings = new NuGetPackSettings {
        Properties = new Dictionary<string, string> { { "Configuration", configuration }},
        Symbols = false,
        NoPackageAnalysis = true,
        Version = versionInfo.NuGetVersionV2,
        OutputDirectory = outputDirectory,
        IncludeReferencedProjects = true
    };
    NuGetPack("./src/Storm.GoogleAnalytics.Reporting/Storm.GoogleAnalytics.Reporting.csproj", settings);
});

Task("Update-AppVeyor-Build-Number")
    .IsDependentOn("Update-Version-Info")
    .WithCriteria(() => AppVeyor.IsRunningOnAppVeyor)
    .Does(() =>
{
    AppVeyor.UpdateBuildVersion(versionInfo.FullSemVer +" | " +AppVeyor.Environment.Build.Number);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Update-Version-Info")
    .IsDependentOn("Update-AppVeyor-Build-Number")
    .IsDependentOn("Build")
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Package")
    ;


Task("Package")
    .IsDependentOn("Build")
    .IsDependentOn("Create-NuGet-Packages");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
