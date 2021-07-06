# devops-targets

[![Nuget downloads](https://img.shields.io/nuget/v/devopstargets.svg)](https://www.nuget.org/packages/DevOpsTargets/)
[![Nuget](https://img.shields.io/nuget/dt/devopstargets)](https://www.nuget.org/packages/DevOpsTargets/)
[![Build status](https://github.com/rosenkolev/devops-targets/actions/workflows/github-actions.yml/badge.svg)](https://github.com/rosenkolev/devops-targets/actions/workflows/github-actions.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/rosenkolev/devops-targets/blob/main/LICENSE)

**This are helper deployment scripts used for building, testing deploying application with .NET script or .NET SDK app.**

Use with dotnet script;
```csharp
#! "net5.0"
#r "nuget:DevOpsTargets"

using System.IO;
using static Bullseye.Targets;
using static DevOpsTargets.Targets;

var pathToApp = Path.Combine(GetScriptFolder(), "MyApp");
Target("check-version", "Check .NET CLI version", () => Shell("dotnet --version"));
Target("build", "Build .NET App", DependsOn("check-version"), () => DotNet.Build(pathToApp));
RunAndExit(Args);
```

## Mathods
```
using static DevOpsTargets.Targets;

// Run all Bullseye Targets with the System.CommandLine library.
void RunAndExit(IEnumerable<string> args, string loggerPrefix);

// Get current script directory
string GetScriptFolder();

// Create directory if not exists
void EnsureDirectoryExists(string path);

// Remove and re-create directory.
void CleanDirectory(string path);

// Delete all passed paths
void DeleteAllFilesAndFolders(params string[] paths);

// Write messages to the output.
void Write(string message, LogLevel logLevel = LogLevel.Message);

// Write messages to the output on a new line.
void WriteLine(string message, LogLevel logLevel = LogLevel.Message);

// Start a shell command for the current OS.
Command ShellCommand(string command, string workingDirectory = null, LogLevel outputLogLevel = LogLevel.Verbose, bool doubleEscapeLinux = true);

// Start a shell command for the current OS and wait result.
string Shell(string command, string workingDirectory = null, LogLevel outputLogLevel = LogLevel.Verbose);

// Start a shell command for the current OS and wait result if test command fail.
string ShellInstall(string testcommand, string installcommand, string workingDirectory = null, LogLevel outputLogLevel = LogLevel.Verbose);

// Merge transform .json into another .json
Transform.TransformSettingsJson(string pathToSettingsJson, string pathToTransformJson); // void

// Find value of a propery in json.
Transform.FindProperyValueInJson(string pathToJson, string propertyName); // string

// Find value in XML by xpath
Transform.GetXmlXPathValue(string pathToXml, string xpath); // string

// Replace ${value} in a text file
Transform.ReplaceInFile(string pathToFile, string pathToNewFile, params EnvValue[] values); // void

// Build csproj file
DotNet.Build(string pathToProject, string configuration = "Debug"); // void

// Publish csproj file
DotNet.Publish(string pathToProject, string pathToOutput, string configuration = "Release"); // void

// Test csproj file
DotNet.Test(string pathToProject, string configuration = "Debug", string pathToOutput = null); // void

// Test with coverage csproj file
DotNet.TestWithCoverage(string pathToProject, string pathToOutput, string configuration = "Debug", TestCoverageFormat formats = TestCoverageFormat.Cobertura, params TestLogInfo[] loggers); // void
```

Also see [samples/](samples/).