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

## Methods
```csharp
using static DevOpsTargets.Targets;

// Run all Bullseye Targets with the System.CommandLine library.
RunAndExit(IEnumerable<string> args, string loggerPrefix);  // void

// Get current script directory
GetScriptFolder();  // string

// Create directory if not exists
EnsureDirectoryExists(string path);  // void

// Remove and re-create directory.
CleanDirectory(string path);  // void

// Delete all passed paths
DeleteAllFilesAndFolders(params string[] paths);  // void

// Write messages to the output.
Write(string message, LogLevel logLevel = LogLevel.Message);  // void

// Write messages to the output on a new line.
WriteLine(string message, LogLevel logLevel = LogLevel.Message);  // void

// Start a shell command for the current OS.
ShellCommand(string command, string workingDirectory = null, LogLevel outputLogLevel = LogLevel.Verbose, bool doubleEscapeLinux = true);  // Command

// Start a shell command for the current OS and wait result.
Shell(string command, string workingDirectory = null, LogLevel outputLogLevel = LogLevel.Verbose);  // string

// Start a shell command for the current OS and wait result if test command fail.
ShellInstall(string testCommand, string installCommand, string workingDirectory = null, LogLevel outputLogLevel = LogLevel.Verbose);  // string

// Merge transform .json into another .json
Transform.TransformSettingsJson(string pathToSettingsJson, string pathToTransformJson);  // void

// Find value of a property in json.
Transform.FindPropertyValueInJson(string pathToJson, string propertyName);  // string

// Find value in XML by xpath
Transform.GetXmlXPathValue(string pathToXml, string xpath);  // string

// Replace ${value} in a text file
Transform.ReplaceInFile(string pathToFile, string pathToNewFile, params EnvValue[] values);  // void

// Build csproj file
DotNet.Build(string pathToProject, string configuration = "Debug");  // void

// Publish csproj file
DotNet.Publish(string pathToProject, string pathToOutput, string configuration = "Release");  // void

// Test csproj file
DotNet.Test(string pathToProject, string configuration = "Debug", string pathToOutput = null);  // void

// Test with coverage csproj file
DotNet.TestWithCoverage(string pathToProject, string pathToOutput, string configuration = "Debug", TestCoverageFormat formats = TestCoverageFormat.Cobertura, params TestLogInfo[] loggers);  // void

// Install NodeJs version if  NodeJs was never installed.
NodeJs.Install(string version);  // void

// Execute `npm install` in folder.
Npm.Install(string pathToFolder);  // void

// Execute `npm ci --no-optional ..` in folder.
Npm.Ci(string pathToFolder, string cacheFolder = null");  // void

// Install npm package globally is not already installed. Example InstallGlobal("@angular/cli");
Npm.InstallGlobal(string package);  // void

// Run NPM command.
Npm.Run(string command, string pathToFolder);  // void
```

Also see [samples/](samples/).
