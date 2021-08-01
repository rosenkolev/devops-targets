#! "net5.0"
#r "nuget:DevOpsTargets, 1.0.0-beta23"

using System;
using System.IO;
using static Bullseye.Targets;
using static DevOps.Targets;

var pathToSolution = Path.GetFullPath(Path.Combine(GetScriptFolder(), ".."));
var pathToLib = Path.Combine(pathToSolution, "src", "DevOps.Targets");
var pathToTests = Path.Combine(pathToSolution, "src", "DevOps.Tests");
var pathToTestResults = Path.Combine(pathToSolution, "TestResults");
var pathToOut = Path.Combine(pathToSolution, "Out");

Target("check-version", "Check .NET CLI version", () => Exec("dotnet --version"));
Target("build", "Build .NET App", DependsOn("check-version"), () => DotNet.Build(pathToLib, "Release"));
Target(
    "test",
    "Build .NET App",
    DependsOn("check-version"),
    () => DotNet.TestWithCoverage(pathToTests, pathToTestResults, "Debug", TestCoverageFormat.Cobertura | TestCoverageFormat.OpenCover, DotNetLoggers.Trx));


Target("publish", () => DotNet.Publish(pathToLib, pathToOut));
Target("default", DependsOn("build", "test", "publish"));
RunAndExit(Args, "Targets");
