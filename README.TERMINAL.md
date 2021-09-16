# DevOpsTargets.Terminal

**This is a wrapper of the native OS terminal that can execute many commands and track command result.**

Example
```csharp
using DevOps.Terminal.Terminals;
// ...
TerminalSingelton.DefaultTerminal.Exec(
	TerminalCommand.CreateParse("echo test"));

// pipe
TerminalSingleton.DefaultTerminal.Exec(
	TerminalCommand.Cd("/src") &&
	TerminalCommand.CreateParse("ls .") &&
	TerminalCommand.CreateParse("ping ..."));

TerminalSingleton.DefaultTerminal.Exec(
	TerminalCommand.Cd("/src") &&
	TerminalCommand.CreateParse("ls .") &&
	TerminalCommand.CreateParse("ping ..."));

var result = TerminalSingleton.DefaultTerminal.ExecuteCommand(
	new [] { "dotnet", "test" });

// result.ExitCode == 0
```
