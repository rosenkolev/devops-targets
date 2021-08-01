using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using DevOps.Terminal.Commands;
using DevOps.Terminal.Loggers;
using DevOps.Terminal.Loggers.Abstraction;
using DevOps.Terminal.Loggers.Host;
using DevOps.Terminal.Terminals.Syntax;

namespace DevOps.Terminal.Terminals
{
    /// <summary>A generic terminal control.</summary>
    public class Terminal
    {
        private readonly Command _command;
        private readonly LogLevel _consoleLogLevel;
        private readonly TerminalMonitor _monitor;
        private readonly TerminalCommandSyntax _syntax;

        /// <summary>Initializes a new instance of the <see cref="Terminal"/> class.</summary>
        public Terminal(TerminalCommandSyntax syntax, LogLevel consoleLogLevel, string workingDirectory)
        {
            var initDirectory = workingDirectory ?? Directory.GetCurrentDirectory();
            var commandLogger = new CommandLogger(consoleLogLevel);

            _syntax = syntax;
            _consoleLogLevel = consoleLogLevel;
            _monitor = new TerminalMonitor(commandLogger);
            _command = new Command(_syntax.CommandName, string.Empty, initDirectory, commandLogger);
            _command.Process.StartInfo.RedirectStandardInput = true;
            _command.Start();
        }

        /// <summary>Initializes a new instance of the <see cref="Terminal"/> class.</summary>
        public Terminal(LogLevel consoleLogLevel, string workingDirectory)
            : this(DefaultTerminalSyntax, consoleLogLevel, workingDirectory)
        {
        }

        /// <summary>Gets the default terminal syntax.</summary>
        public static TerminalCommandSyntax DefaultTerminalSyntax =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                new WindowsCmdSyntax() :
                new UnixShSyntax();

        /// <summary>Executes the specified command.</summary>
        public CommandResult Exec(TerminalCommand command) =>
            command.HasNext ?
                ExecPipe(command) :
                ExecuteCommand(command.CommandArguments, command.LogLevel);

        /// <summary>Executes the specified command.</summary>
        public CommandResult ExecPipe(TerminalCommand command)
        {
            var builder = new StringBuilder();
            CommandResult result = null;
            TerminalCommand currentCommand = command;
            do
            {
                if (result != null)
                {
                    builder.Append(' ');
                }

                result = ExecuteCommand(currentCommand.CommandArguments, currentCommand.LogLevel);
                builder.Append(result.Output);
                currentCommand = command.GetNext();
            }
            while (result.ExitCode == 0 && currentCommand != null);

            return new CommandResult(builder.ToString(), result.ExitCode);
        }

        /// <summary>Closes this terminal.</summary>
        public void Close()
        {
            _command.Process.Close();
            _command.Dispose();
        }

        /// <summary>Executes the command.</summary>
        public CommandResult ExecuteCommand(string[] commandArguments, LogLevel? logLevel = null, bool raw = false)
        {
            _monitor.SetLogLevel(logLevel ?? _command.Logger.LogLevel);

            var result = raw ?
                Execute(new TerminalExecution(string.Join(" ", commandArguments))) :
                Execute(commandArguments);

            _monitor.TextOutput.Reset();
            _monitor.SetLogLevel(_consoleLogLevel);

            return result;
        }

        /// <summary>Executes the command.</summary>
        internal CommandResult Execute(params string[] commandArguments)
        {
            var commandText = _syntax.BuildCommand(commandArguments);
            return Execute(new TerminalExecution(commandText));
        }

        /// <summary>Executes the command.</summary>
        internal CommandResult Execute(TerminalExecution execution)
        {
            _monitor.WriteHostLine(execution.Command, LogLevel.Debug);

            var prefix = execution.Prefix;
            var statusCodeCommand = "echo " + prefix + _syntax.ReturnCodeCommand;

            _command.Process.StandardInput.WriteLine(execution.Command);
            _command.Process.StandardInput.WriteLine(statusCodeCommand);

            var skipLines = _syntax.BuildInputClearWildCards(execution.Command, statusCodeCommand).ToArray();
            var outputResult = _monitor.WaitForResult(prefix + '*', skipLines);
            var code = outputResult.Substring(prefix.Length);
            var statusCode = Convert.ToInt32(code);
            var output = _monitor.Output.Trim(' ', '\r', '\n');
            var result = new CommandResult(output, statusCode);

            _monitor.WriteHostLine("Exit code " + code, LogLevel.Debug);

            return result;
        }

            /// <summary>Command logger monitor.</summary>
        internal class TerminalMonitor : CommandMonitor
        {
            private readonly CommandLogger _commandLogger;

            /// <summary>Initializes a new instance of the <see cref="TerminalMonitor"/> class.</summary>
            public TerminalMonitor(CommandLogger commandLogger)
                : base(commandLogger)
            {
                _commandLogger = commandLogger;
                TextOutput = new TextOutput();
                HostOutput = Out.CreateConsoleOutput();
            }

            /// <summary>Gets the output.</summary>
            public string Output => TextOutput.Logger.Output;

            /// <summary>Gets the host output.</summary>
            public HostOutput HostOutput { get; init; }

            /// <summary>Gets the text output.</summary>
            public TextOutput TextOutput { get; init; }

            /// <summary>Sets the log level.</summary>
            public void SetLogLevel(LogLevel logLevel) =>
                _commandLogger.LogLevel = logLevel;

            /// <summary>Waits for exit result.</summary>
            public string WaitForResult(string endMonitorWildcard, string[] skipLinesWildcards) =>
                WaitForResult(endMonitorWildcard, skipLinesWildcards, WriteLine);

            /// <summary>Writes the host line.</summary>
            public void WriteHostLine(string message, LogLevel logLevel) =>
                HostOutput.WriteLine(message, logLevel);

            private void WriteLine(OutputMessage output)
            {
                TextOutput.WriteLine(output.Message, output.Level);
                HostOutput.WriteLine(output.Message, output.Level);
            }
        }
    }
}
