using System;
using System.Collections.Generic;

using DevOps.Terminal.Commands;
using DevOps.Terminal.Loggers.Abstraction;

namespace DevOps.Terminal.Terminals;

/// <summary>Terminal execute command.</summary>
public class TerminalCommand
{
    private readonly TerminalCommandExecuteInfo _runInfo;
    private Queue<TerminalCommand> _pipe;

    /// <summary>Initializes a new instance of the <see cref="TerminalCommand"/> class.</summary>
    public TerminalCommand(string[] commandArguments, LogLevel? logLevel, Action<CommandResult> onComplete)
        : this(
              new TerminalCommandExecuteInfo
              {
                  CommandArguments = commandArguments,
                  LogLevel = logLevel,
                  OnComplete = onComplete,
              })
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TerminalCommand"/> class.</summary>
    public TerminalCommand(TerminalCommandExecuteInfo runInfo)
    {
        _runInfo = runInfo;
    }

    /// <summary>Gets the command arguments.</summary>
    public string[] CommandArguments =>
        _runInfo.CommandArguments;

    /// <summary>Gets the execution information.</summary>
    public TerminalCommandExecuteInfo Info => _runInfo;

    /// <summary>Gets a value indicating whether this instance has next command.</summary>
    public bool HasNext => _pipe != null && _pipe.Count > 0;

    /// <summary>Implements the operator op_BitwiseAnd.</summary>
    public static TerminalCommand operator &(TerminalCommand first, TerminalCommand second)
    {
        first._pipe ??= new Queue<TerminalCommand>();

        first._pipe.Enqueue(second);
        return first;
    }

    /// <summary>Creates the specified command arguments.</summary>
    public static TerminalCommand Create(params string[] commandArguments) =>
        new TerminalCommand(commandArguments, null, null);

    /// <summary>Parses the specified command.</summary>
    public static TerminalCommand CreateParse(string command, LogLevel? logLevel) =>
        CreateParse(command, logLevel, null);

    /// <summary>Parses the specified command.</summary>
    public static TerminalCommand CreateParse(string command, LogLevel? logLevel, Action<CommandResult> onComplete) =>
        new TerminalCommand(
            command.Split(' ', StringSplitOptions.RemoveEmptyEntries),
            logLevel,
            onComplete);

    /// <summary>Creates the change directory command.</summary>
    public static TerminalCommand Cd(string workingFolder) =>
        new TerminalCommand(
            new[] { "cd", workingFolder },
            LogLevel.Debug,
            null);

    /// <summary>Gets the next command in the pipe.</summary>
    public TerminalCommand GetNext() => HasNext ? _pipe.Dequeue() : null;
}
