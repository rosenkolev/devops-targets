using System;

using DevOps.Terminal.Commands;
using DevOps.Terminal.Loggers.Abstraction;

namespace DevOps.Terminal.Terminals;

/// <summary>The terminal command execution information.</summary>
public sealed class TerminalCommandExecuteInfo
{
    /// <summary>Gets the command arguments.</summary>
    public string[] CommandArguments { get; init; }

    /// <summary>Gets the log level.</summary>
    public LogLevel? LogLevel { get; init; }

    /// <summary>Gets the on complete action.</summary>
    public Action<CommandResult> OnComplete { get; init; }
}
