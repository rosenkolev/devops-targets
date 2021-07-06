using System;
using System.Runtime.InteropServices;

namespace DevOpsTargets
{
    /// <summary>Targets main class.</summary>
    public static partial class Targets
    {
        /// <summary>Execute a shell command in any OS.</summary>
        public static Command ShellCommand(
            string command,
            string workingDirectory = null,
            LogLevel outputLogLevel = LogLevel.Verbose,
            bool doubleEscapeLinux = true)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var cmd = doubleEscapeLinux ? command.Replace("\\", "\\\\").Replace("\"", "\\\"") : command;
                return Command.CreateAndWait("/bin/sh", "-c \"" + cmd + "\"", workingDirectory, outputLogLevel);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Command.CreateAndWait("cmd.exe", "/C " + command, workingDirectory, outputLogLevel);
            }

            throw new InvalidOperationException("Not supported OS.");
        }

        /// <summary>Execute a shell command in any OS.</summary>
        public static string Shell(string command, string workingDirectory = null, LogLevel outputLogLevel = LogLevel.Verbose)
        {
            var com = ShellCommand(command, workingDirectory, outputLogLevel);
            com.FailWhenExitCode(0);
            return com.Output;
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
