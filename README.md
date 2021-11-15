# devops-targets

[![Nuget downloads](https://img.shields.io/nuget/v/devopstargets.svg)](https://www.nuget.org/packages/DevOpsTargets/)
[![Nuget](https://img.shields.io/nuget/dt/devopstargets)](https://www.nuget.org/packages/DevOpsTargets/)
[![Build status](https://github.com/rosenkolev/devops-targets/actions/workflows/github-actions.yml/badge.svg)](https://github.com/rosenkolev/devops-targets/actions/workflows/github-actions.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/rosenkolev/devops-targets/blob/main/LICENSE)

**This are helper deployment scripts used for building, testing deploying application with .NET script or .NET SDK app.**

Use with dotnet script;
```csharp
#! "net6.0"
#r "nuget:DevOpsTargets"

using System.IO;
using static Bullseye.Targets;
using static DevOpsTargets.Targets;

var pathToApp = Path.Combine(GetScriptFolder(), "MyApp");
Target("check-version", "Check .NET CLI version", () => Exec("dotnet --version"));
Target("build", "Build .NET App", DependsOn("check-version"), () => WriteLine("Do some work"));

// Run all Bullseye Targets with the System.CommandLine library.
RunAndExit(Args, loggerPrefix: "My App");
```

## Methods

### Write Messages
```csharp
    using static DevOpsTargets.Targets;
    
    // Set default max log level. See [DevOps.Terminal](./README.TERMINAL.md)
    // DevOps.Terminal.Out.MaxLogLevel = LogLevel.Verbose;

    // Write a text to the output
    Write(string message, LogLevel.Message);
    Write(string message, LogLevel.Debug);
    Write(string message); // Default to Message
    
    // Write messages to the output on a new line.
    WriteLine(string message, LogLevel.Error);
    WriteLine(string message);
```

### Execute Commands

All commands are executed in the default system terminal. In cmd.exe for Windows and /bin/sh for Mac and Linux.

```csharp
    using static DevOpsTargets.Targets;

    // Start a new command (don't wait for result). Only first argument is required. See DevOps.Terminal.Commands.Command.CreateAndStart.
    var command = CommandStart("sleep 13", workingDirectory: "/usr", outputLogLevel: LogLevel.Verbose);
    var command = CommandStart("echo Test");
    
    // Start a new command and wait for result. Verify exit code.
    var commandResult = CommandExec("sleep 13", workingDirectory: null, outputLogLevel: LogLevel.Verbose, validExitCode: 0);

    // Exec(...) executes all commands in the same terminal instance. See DevOps.Terminal.Terminals.TerminalSingleton.DefaultTerminal.
    // Example (Windows):
    // 
    // Exec("set TEST=1");
    // Exec("echo %TEST%"); // 1

    // Execute a command in a specific directory. Wait for the result.
    var commandResult = Exec("dir", workingDirectory: "c:\\");

    // Execute a command and output the result as Message. Do not validate exit code.
    var commandResult = Exec("dir", LogLevel.Message, validExitCode: null);
    commandResult.Output; // output
    commandResult.ExitCode; // 0

    // The same as the above. See DevOps.Terminal.Terminals.Terminal.Exec(...);
    Exec(TerminalCommand.CreateParse("dir", logLevel: HostLogLevel.Message), validExitCode: null);
```

### Packages and Installers
```csharp
    using static DevOpsTargets.Targets;

    // Install NodeJs version. Both Windows and Linux.
    NodeJs.Install("14.17.1");

    // Find NodeJs and set NODE_PATH if not set.
    NodeJs.SetNodePath();

    // Install Java jdk, If not installed.
    Java.Install("openjdk@1.15.0");

    // Install NPM package, globally.
    Npm.InstallGlobal("@angular/cli");

    // Run 'npm install' in the provided folder.
    Npm.Install("c:\\my-project");

    // Run 'npm ci' in the folder. Provide cache folder path if needed.
    Npm.Ci("c:\\my-project", cacheFolder: null);

    // Run a npm command.
    Npm.Run("test", pathToFolder: "c:\\my=project");
```

### Files and Directories
```csharp
    using static DevOpsTargets.Targets;

    // Gets the calling asembly or c# script folder path.
    var currentPath = GetScriptFolder();

    // Ensure the directory exists.
    EnsureDirectoryExists("c:\\Temp");

    // Deletes all provided files and folders as params.
    DeleteAllFilesAndFolders("c:\\Temp", "d:\\image.png" /*, ...*/);

    // Deletes and re-create the folder.
    CleanDirectory("c:\\publish");

    // Get a temp system filename. Example: "c:\Users\my-user\AppData\Local\Temp\72f69d35e54b40e29d9849b71e43a7dc.png".
    var tempFileName = GetTempFileName(".png");

    // Create multi-line content from lines as arguments.
    var content = GetContentFromLines("Line 1", "Line 2");
```

### .NET and Transforms
```csharp
    using static DevOpsTargets.Targets;

    // Merge transform .json into another .json
    Transform.TransformSettingsJson(pathToSettingsJson, pathToTransformJson);

    // Find value of a property in json.
    string value = Transform.FindPropertyValueInJson(pathToJson, propertyName);

    // Find value in XML by xpath
    string value = Transform.GetXmlXPathValue(pathToXml, xpath);

    // Replace ${value} in a text file
    Transform.ReplaceInFile(pathToFile, pathToNewFile, params EnvValue[] values);

    // Install .NET SDK when missing
    DotNet.Sdk.Install("5.0");

    // Build csproj file
    DotNet.Build(pathToCsProj, configuration: "Debug");

    // Publish csproj file
    DotNet.Publish(pathToCsProj, pathToOutput, configuration: "Release");

    // Test csproj file
    DotNet.Test(string pathToCsProj, configuration: "Debug", pathToTrxOutput: null);

    // Test with coverage csproj file
    DotNet.TestWithCoverage(pathToCsProject, pathToTestResults, configuration: "Debug", formats: TestCoverageFormat.Cobertura, DotNetLoggers.JUnit);
```

### SonarScanner
```csharp
    using static DevOpsTargets.Targets;

    // Run sonar scanner. See the extended example.
    Sonarqube.RunScanner(() => DotNet.Build(solutionDir), solutionDir, key: "my-key", version: "1.0.0", branch: "main");
```

Also see [samples/](samples/).
