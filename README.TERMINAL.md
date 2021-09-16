# DevOpsTargets.Terminal

**This is a wrapper of the native OS terminal that can execute many commands and track command result.**

Example
```csharp
using DevOps.Terminal.Terminals;
// ...
TerminalSingelton.DefaultTerminal.Exec(
	TerminalCommand.CreateParse("echo test"));

// pipe
TerminalSingelton.DefaultTerminal.Exec(
	TerminalCommand.Cd("/src") &&
	TerminalCommand.CreateParse("ls .") &&
	TerminalCommand.CreateParse("ping ..."));

TerminalSingelton.DefaultTerminal.Exec(
	TerminalCommand.Cd("/src") &&
	TerminalCommand.CreateParse("ls .") &&
	TerminalCommand.CreateParse("ping ..."));

var result = TerminalSingelton.DefaultTerminal.ExecuteCommand(
	new [] { "dotnet", "test" });

// result.ExitCode == 0
```
