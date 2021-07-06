#! "net5.0"
#r "nuget:System.CommandLine, 2.0.0-beta1.21308.1"
#r "nuget:Bullseye, 3.7.0"
#r "..\src\DevOpsTargets\bin\Debug\net5.0\DevOpsTargets.dll"

using System;
using System.IO;
using static Bullseye.Targets;
using static DevOpsTargets.Targets;

var pathToSolution = Path.GetFullPath(Path.Combine(GetScriptFolder(), ".."));
var pathToLib = Path.Combine(pathToSolution, "src", "DevOpsTargets");
var pathToTests = Path.Combine(pathToSolution, "src", "DevOpsTargets.Tests");
var pathToTestResults = Path.Combine(pathToSolution, "TestResults");
var pathToOut = Path.Combine(pathToSolution, "Out");

Target("check-version", "Check .NET CLI version", () => Shell("dotnet --version"));
Target("build", "Build .NET App", DependsOn("check-version"), () => DotNet.Build(pathToLib, "Release"));

Target(
    "test",
    "Build .NET App",
    DependsOn("check-version"),
    () => DotNet.TestWithCoverage(pathToTests, pathToTestResults, "Debug", TestCoverageFormat.Cobertura | TestCoverageFormat.OpenCover, DotNetLoggers.Trx));

Target("publish", () => DotNet.Publish(pathToLib, pathToOut));
Target("default", DependsOn("build", "test", "publish"));
RunAndExit(Args, "Targets");
