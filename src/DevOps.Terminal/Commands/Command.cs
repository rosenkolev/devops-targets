using System;
using System.Diagnostics;

using DevOps.Terminal.Loggers;
using DevOps.Terminal.Loggers.Abstraction;

namespace DevOps.Terminal.Commands
{
    /// <summary>An external command.</summary>
    public class Command : IDisposable
    {
        /// <summary>Initializes a new instance of the <see cref="Command"/> class.</summary>
        public Command(string commandPath, string arguments, string workingDirectory, ICommandLogger logger)
            : this(ProcessStartInfoFactory.Create(commandPath, arguments, workingDirectory), logger)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="Command"/> class.</summary>
        public Command(ProcessStartInfo startInfo, ICommandLogger logger)
            : this(ProcessFactory.Create(startInfo, logger), logger)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="Command"/> class.</summary>
        public Command(Process process, ICommandLogger logger)
        {
            Process = process;
            Logger = logger;
        }

        /// <summary>Gets the logger.</summary>
        public ICommandLogger Logger { get; init; }

        /// <summary>Gets the exit code.</summary>
        public int ExitCode => Process.ExitCode;

        /// <summary>Gets a value indicating whether this command has exited.</summary>
        public bool HasExited => Process.HasExited;

        /// <summary>Gets the text output.</summary>
        public string TextOutput => Logger.FindOutput<TextOutput>()?.Logger.Output;

        /// <summary>Gets the process.</summary>
        internal Process Process { get; init; }

        /// <summary>Execute command and start it.</summary>
        public static Command CreateAndStart(
            string commandPath,
            string arguments,
            string workingDirectory = null,
            LogLevel outputLogLevel = LogLevel.Debug)
        {
            var logger = new CommandLogger(outputLogLevel, Out.CreateConsoleOutput(), new TextOutput());
            var command = new Command(commandPath, arguments, workingDirectory, logger);

            command.Start();

            return command;
        }

        /// <summary>Execute command and wait for exit.</summary>
        public static Command CreateAndWait(
            string commandPath,
            string arguments,
            string workingDirectory = null,
            LogLevel outputLogLevel = LogLevel.Debug)
        {
            var command = CreateAndStart(commandPath, arguments, workingDirectory, outputLogLevel);

            command.WaitForResult();

            return command;
        }

        /// <summary>Closes the specified command.</summary>
        public void Close()
        {
            if (!Process.HasExited)
            {
                Process.Close();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Wait for the process to end end return the output as string.</summary>
        public void WaitForResult()
        {
            Process.WaitForExit();
            Out.WriteLine($"Process[{Process.Id}]: Exit code '{ExitCode}'", LogLevel.Debug);
        }

        /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
                Process.ErrorDataReceived -= Logger.LogError;
                Process.OutputDataReceived -= Logger.LogOutput;
                Process.Dispose();
            }
        }
    }
}
