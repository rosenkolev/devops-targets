using DevOps.Terminal.Commands;
using DevOps.Terminal.Terminals;

using DevOpsTerminal = DevOps.Terminal.Terminals.Terminal;
using HostLogLevel = DevOps.Terminal.Loggers.Abstraction.LogLevel;

namespace DevOps
{
    /// <summary>Targets main class.</summary>
    public static partial class Targets
    {
        /// <summary>Execute a shell command in any OS.</summary>
        public static Command ShellStart(
            string command,
            string workingDirectory = null,
            LogLevel outputLogLevel = LogLevel.Verbose)
        {
            var syntax = DevOpsTerminal.DefaultTerminalSyntax;
            var cmd = TerminalCommand.CreateParse(command, null);
            var args = syntax.BuildCommand(cmd.CommandArguments);
            var commandArgs = string.Format(syntax.CommandArguments, args);
            return Command.CreateAndStart(syntax.CommandName, commandArgs, workingDirectory, (HostLogLevel)outputLogLevel);
        }

        /// <summary>Execute a shell command in any OS.</summary>
        public static Command ShellCommand(
            string command,
            string workingDirectory = null,
            LogLevel outputLogLevel = LogLevel.Verbose)
        {
            var cmd = ShellStart(command, workingDirectory, outputLogLevel);
            cmd.WaitForResult();
            return cmd;
        }

        /// <summary>Execute a shell command in any OS.</summary>
        public static CommandResult ExecRaw(string command, int? validExitCode = 0)
        {
            var result = TerminalSingelton.DefaultTerminal.ExecuteCommand(new[] { command }, null, true);
            if (validExitCode.HasValue)
            {
                result.ThrowOnExitCode(validExitCode.Value);
            }

            return result;
        }

        /// <summary>Execute a shell command in a terminal.</summary>
        public static CommandResult ExecInTerminal(TerminalCommand command, DevOpsTerminal terminal, int? validExitCode = 0)
        {
            var result = terminal.Exec(command);
            if (validExitCode.HasValue)
            {
                result.ThrowOnExitCode(validExitCode.Value);
            }

            return result;
        }

        /// <summary>Execute a shell command in any OS.</summary>
        public static CommandResult Exec(TerminalCommand command, int? validExitCode = 0) =>
            ExecInTerminal(command, TerminalSingelton.DefaultTerminal, validExitCode);

        /// <summary>Execute a shell command in any OS.</summary>
        public static CommandResult Exec(string script, LogLevel? logLevel = null, int? validExitCode = 0) =>
            Exec(
                TerminalCommand.CreateParse(script, logLevel.HasValue ? (HostLogLevel)logLevel.Value : null),
                validExitCode);

        /// <summary>Execute a shell command in any OS.</summary>
        public static CommandResult Exec(string script, string workingDirectory) =>
            Exec(
                TerminalCommand.Cd(workingDirectory) &
                TerminalCommand.CreateParse(script, null));

        /// <summary>Execute a shell command in any OS.</summary>
        public static string Shell(string command, string workingDirectory = null, LogLevel outputLogLevel = LogLevel.Verbose)
        {
            var com = ShellCommand(command, workingDirectory, outputLogLevel);
            com.FailWhenExitCode(0);
            return com.TextOutput;
        }

        /// <summary>Execute the install command, in case the test command fail.</summary>
        public static string ShellInstall(string testcommand, string installcommand, string workingDirectory = null, LogLevel outputLogLevel = LogLevel.Verbose)
        {
            var command = ShellCommand(testcommand, workingDirectory, outputLogLevel);
            if (command.ExitCode == 0)
            {
                return null;
            }

            return Shell(installcommand, workingDirectory);
        }
    }
}
