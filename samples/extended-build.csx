#! "net5.0"
#r "nuget:DevOpsTargets, 1.2.0"

using System.IO;
using System.IO.Compression;
using DevOps.Packages;
using static Bullseye.Targets;
using static DevOps.Targets;

var projectName = "ProjectName";
var projectApiName = projectName + ".Presentation.Api";
var solutionDir = GetScriptFolder();
var pathToWebApp = Path.Combine(solutionDir, projectName + ".Presentation.Web", "ClientApp");
var pathToApi = Path.Combine(solutionDir, projectApiName);
var pathToAdmin = Path.Combine(solutionDir, projectName + ".Presentation.Admin");
var pathToTests = Path.Combine(solutionDir, projectName + ".Tests");
var pathToTools = Path.Combine(solutionDir, "tools");
var pathToTestResult = Path.Combine(solutionDir, "TestResults");
var pathToApiProject = Path.Combine(pathToApi, projectApiName + ".csproj");
var pathToApiPublish = Path.Combine(solutionDir, "publish", projectApiName);
var pathToNodeCache = Path.Combine(solutionDir, ".npm");

Option("--branch", "The current source branch; Default: main", "main");
Option<string>("--sonarqube", "The sonarqube login key");
Option<string>("--settings", "The appsettings transform file");
Target("api-build", "Build Api for Release", () => DotNet.Build(pathToApi, "Release"));
Target("admin-build", "Build Admin for Release", () => DotNet.Build(pathToAdmin, "Release"));
Target("node-install", "NodeJs Install", () => NodeJs.Install("14.17.1"));
Target("web-restore", "Web Install NPM Packages", DependsOn("node-install"), () => Npm.Ci(pathToWebApp, pathToNodeCache));
Target("web-build", "Web Build for Production", DependsOn("web-restore"), () => Npm.Run("build", pathToWebApp));
Target("web-lint", "Web Lint", DependsOn("web-restore"), () => Npm.Run("lint", pathToWebApp));
Target("web-test", "Web Unit Tests", DependsOn("web-restore"), () => Npm.Run("test", pathToWebApp));
Target("dotnet-test", "Unit Tests", () => DotNet.TestWithCoverage(pathToTests, pathToTestResult + "/", "Debug", TestCoverageFormat.Cobertura | TestCoverageFormat.OpenCover, DotNetLoggers.Trx, DotNetLoggers.JUnit));
Target("sonarscanner-install", () => Exec("dotnet tool install -g dotnet-sonarscanner", validExitCode: null));
Target("jrk-install", () => Java.Install("openjdk@1.15.0"));
Target("dotnet-sdk3-install", "Install .NET SDK 3.1", () => DotNet.Sdk.Install("3.1"));
Target("build-for-scan", "Build solution for dependency scan", () => {
    DotNet.Sdk.SetMsBuildSdksPath();
    DotNet.Build(pathToAdmin, "Debug");
    DotNet.Build(pathToApi, "Debug");
    DotNet.Build(pathToTests, "Debug");
});
Target("scan", "OWASP Dependency Scan", DependsOn("dotnet-sdk3-install", "jrk-install", "build-for-scan"),() => {
    var executePath = InstallDependencyCheck("6.3.1");
    EnsureDirectoryExists(pathToTestResult);
    Exec(TerminalCommand.Cd(solutionDir));
    Exec($"{executePath} --project {projectName} --scan \"**/bin/Debug/net5.0/**/*.dll\" --dotnet /usr/bin/dotnet -f JSON -f HTML -o TestResults/");
});

Target("sonarqube", "SonarQube Analysis", DependsOn("sonarscanner-install", "node-install", "jrk-install"), () => {
    var version = Transform.GetXmlXPathValue(pathToApiProject, "/PropertyGroup/Version");
    var sdk = Transform.GetXmlXPathValue(pathToApiProject, "/PropertyGroup/TargetFramework");
    var branch = ValueForOption<string>("--branch");
    var sonarqube = ValueForOption<string>("--sonarqube");
    if (string.IsNullOrEmpty(branch) || branch == "main") {
        branch = "master";
    }

    Sonarqube.TransformGlobalSettings(
        "/root/.dotnet/tools/.store/dotnet-sonarscanner",
         pathToTools,
         sdk,
         new EnvValue("$BASE_PATH", solutionDir),
         new EnvValue("$LOGIN_KEY", sonarqube));

    WriteLine("Install typescript 3.5.3. For eslint analysis.");
    NodeJs.SetNodePath();
    Npm.InstallGlobal("typescript@3.5.3");

    WriteLine("Run sonarscanner.");
    Sonarqube.RunScanner(() => DotNet.Build(), solutionDir, key: "dotnet-templates", version: version, branch: branch);
});

Target("api-publish-build", "Publish for Release", () => DotNet.Publish(pathToApi, pathToApiPublish));
Target("api-publish", "Transform appsettings", DependsOn("api-publish-build"), () => {
    var pathToTransformSettings = ValueForOption<string>("--settings");
    var pathToApiPublishSettings = Path.Combine(pathToApiPublish, "appsettings.json");
    Transform.TransformSettingsJson(pathToApiPublishSettings, pathToTransformSettings);
});

RunAndExit(Args, projectName);

string InstallDependencyCheck(string version)
{
    var directoryPath = Downloader.GetToolPath("dependency-check", version, string.Empty);
    var executePath = Path.Combine(directoryPath, "dependency-check", "bin", "dependency-check.sh");
    if (!Directory.Exists(directoryPath))
    {
        var path = Downloader.DownloadFile("dependency-check", version, "dependency-check.zip", $"https://github.com/jeremylong/DependencyCheck/releases/download/v{version}/dependency-check-{version}-release.zip");
        ZipFile.ExtractToDirectory(path, directoryPath);
        Exec("chmod 755 " + executePath);
    }

    return executePath;
}
