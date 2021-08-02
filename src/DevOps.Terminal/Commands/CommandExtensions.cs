using System;

using DevOps.Terminal.Loggers;
using DevOps.Terminal.Loggers.Abstraction;

namespace DevOps.Terminal.Commands
{
    /// <summary>Command extensions.</summary>
    public static class CommandExtensions
    {
        /// <summary>Assert the process exit code is not the allowed exit code.</summary>
        public static void FailWhenExitCode(this Command command, int allowedExitCode) =>
            ValidateExitCode(command, code => code == allowedExitCode, "Check if exit code is " + allowedExitCode);

        /// <summary>Validates the command exit code.</summary>
        public static void ValidateExitCode(this Command command, Func<int, bool> handleExitCode, string message = "Check if exit code is alloed") =>
            Guard.IsTrue(command.HasExited && handleExitCode(command.ExitCode), () => command.Logger.FindOutput<TextOutput>()?.Logger.Error, message);

        /// <summary>Validates the command exit code.</summary>
        public static void ThrowOnExitCode(this CommandResult command, int exitCode)
        {
            if (command.ExitCode != exitCode)
            {
                throw new ExitCodeException(exitCode);
            }
        }

        /// <summary>Finds the logger output.</summary>
        /// <typeparam name="T">The type of the output.</typeparam>
        public static T FindOutput<T>(this ICommandLogger logger)
            where T : class, IOutput =>
            logger is CommandLogger commandLogger ?
            (T)commandLogger.Outputs.Find(o => o is T) :
            null;

        /// <summary>Starts the specified command.</summary>
        public static void Start(this Command command)
        {
            var info = command.Process.StartInfo;
            Out.WriteLine($"{info.FileName} {info.Arguments}", LogLevel.Debug);

            if (command.Process.Start())
            {
                if (command.Process.StartInfo.RedirectStandardOutput)
                {
                    command.Process.BeginOutputReadLine();
                }

                if (command.Process.StartInfo.RedirectStandardError)
                {
                    command.Process.BeginErrorReadLine();
                }
            }

            Out.WriteLine("----------------------", command.Logger.LogLevel);
        }
    }
}
