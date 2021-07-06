using System;
using System.Diagnostics;
using System.Text;

using static DevOpsTargets.Targets;

namespace DevOpsTargets
{
    /// <summary>An external command.</summary>
    public class Command : System.IDisposable
    {
        private readonly StringBuilder _lastStandardErrorOutput = new StringBuilder();
        private readonly StringBuilder _lastStandardOutput = new StringBuilder();
        private readonly Process _process = new Process();

        /// <summary>Initializes a new instance of the <see cref="Command"/> class.</summary>
        public Command(string commandPath, string arguments, string workingDirectory, LogLevel outputLogLevel = LogLevel.Verbose)
        {
            WriteLine($"{commandPath} {arguments}", LogLevel.Debug);

            _process.StartInfo = new ProcessStartInfo(commandPath)
            {
                CreateNoWindow = true,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };

            if (!string.IsNullOrEmpty(workingDirectory))
            {
                _process.StartInfo.WorkingDirectory = workingDirectory;
            }

            _process.ErrorDataReceived += (s, e) => Log(_lastStandardErrorOutput, e.Data, outputLogLevel);
            _process.OutputDataReceived += (s, e) => Log(_lastStandardOutput, e.Data, outputLogLevel);
            _process.Start();
            _process.BeginErrorReadLine();
            _process.BeginOutputReadLine();

            WriteLine("----------------------", outputLogLevel);
        }

        /// <summary>Gets the exit code.</summary>
        public int ExitCode => _process.ExitCode;

        /// <summary>Gets the standard output.</summary>
        public string Output => _lastStandardOutput.ToString().Trim();

        /// <summary>Execute command and wait for exit.</summary>
        public static Command CreateAndWait(string commandPath, string arguments, string workingDirectory = null, LogLevel outputLogLevel = LogLevel.Verbose)
        {
            var command = new Command(commandPath, arguments, workingDirectory, outputLogLevel);

            command.WaitForResult();

            WriteLine("Exit with status code " + command.ExitCode, LogLevel.Debug);

            return command;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Assert the process exit code is not the allowed exit code.</summary>
        public void FailWhenExitCode(int allowedExitCode) =>
            Guard.Truthy(_process.ExitCode == allowedExitCode, _lastStandardErrorOutput.ToString().Trim(), "Check if exit code is " + allowedExitCode);

        /// <summary>Wait for the process to end end return the output as string.</summary>
        public void WaitForResult() => _process.WaitForExit();

        /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _process.Dispose();
            }
        }

        private static void Log(StringBuilder output, string message, LogLevel level)
        {
            var msg = message?.TrimEnd() ?? string.Empty;
            WriteLine(msg, level);
            output.AppendLine(msg);
        }
    }
}
