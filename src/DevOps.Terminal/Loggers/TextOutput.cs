using System;
using System.Text;

using DevOps.Terminal.Loggers.Abstraction;

namespace DevOps.Terminal.Loggers
{
    /// <summary>A string logger.</summary>
    public sealed class TextOutput : IOutput
    {
        /// <summary>Gets the logger.</summary>
        public ScopeLogger Logger { get; private set; } = new ScopeLogger();

        /// <inheritdoc/>
        public void Write(string message, LogLevel logLevel)
        {
            if (logLevel == LogLevel.Error)
            {
                Logger.LogError(message);
            }
            else
            {
                Logger.LogMessage(message);
            }
        }

        /// <inheritdoc/>
        public void WriteLine() =>
            Write(Environment.NewLine, LogLevel.Message);

        /// <inheritdoc/>
        public void WriteLine(string message, LogLevel logLevel) =>
            Write(message + Environment.NewLine, logLevel);

        /// <summary>Resets this output.</summary>
        public void Reset() =>
            Logger = new ScopeLogger();

        /// <summary>A scope logger.</summary>
        public class ScopeLogger
        {
            private readonly StringBuilder _lastStandardOutput = new StringBuilder();
            private readonly StringBuilder _lastStandardError = new StringBuilder();

            /// <summary>Gets the error.</summary>
            public string Error => _lastStandardError.ToString().Trim();

            /// <summary>Gets the output.</summary>
            public string Output => _lastStandardOutput.ToString().Trim();

            /// <summary>Logs the error.</summary>
            public void LogError(string message) => _lastStandardError.Append(message);

            /// <summary>Logs the message.</summary>
            public void LogMessage(string message) => _lastStandardOutput.Append(message);
        }
    }
}
