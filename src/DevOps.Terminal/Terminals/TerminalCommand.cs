using System;
using System.Collections.Generic;

using DevOps.Terminal.Loggers.Abstraction;

namespace DevOps.Terminal.Terminals
{
    public class TerminalCommand
    {
        private Queue<TerminalCommand> _pipe;

        /// <summary>Initializes a new instance of the <see cref="TerminalCommand"/> class.</summary>
        public TerminalCommand(string[] commandArguments, LogLevel? logLevel)
        {
            CommandArguments = commandArguments;
            LogLevel = logLevel;
        }

        /// <summary>Gets the command arguments.</summary>
        public string[] CommandArguments { get; init; }

        /// <summary>Gets the log level.</summary>
        public LogLevel? LogLevel { get; init; }

        /// <summary>Gets a value indicating whether this instance has next command.</summary>
        public bool HasNext => _pipe != null && _pipe.Count > 0;

        /// <summary>Implements the operator op_BitwiseAnd.</summary>
        public static TerminalCommand operator &(TerminalCommand first, TerminalCommand second)
        {
            if (first._pipe == null)
            {
                first._pipe = new Queue<TerminalCommand>();
            }

            first._pipe.Enqueue(second);
            return first;
        }

        /// <summary>Creates the specified command arguments.</summary>
        public static TerminalCommand Create(params string[] commandArguments) =>
            new TerminalCommand(commandArguments, null);

        /// <summary>Parses the specified command.</summary>
        public static TerminalCommand CreateParse(string command, LogLevel? logLevel) =>
            new TerminalCommand(command.Split(' ', StringSplitOptions.RemoveEmptyEntries), logLevel);

        /// <summary>Creates the change directory command.</summary>
        public static TerminalCommand Cd(string workingFolder) =>
            new TerminalCommand(new[] { "cd", workingFolder }, Loggers.Abstraction.LogLevel.Debug);

        /// <summary>Gets the next command in the pipe.</summary>
        public TerminalCommand GetNext() => HasNext ? _pipe.Dequeue() : null;
    }
}
