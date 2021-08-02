using System;
using System.IO;
using System.Linq;

using DevOps.Terminal.Loggers.Abstraction;

namespace DevOps.Terminal.Loggers.Host
{
    /// <summary>A host writer class.</summary>
    public sealed class HostOutput : IOutput
    {
        private readonly string _prefix;
        private readonly TextWriter _writer;
        private readonly HostPalette _p;
        private readonly LogLevel _maxLogLevel;

        /// <summary>Initializes a new instance of the <see cref="HostOutput"/> class.</summary>
        public HostOutput(TextWriter writer, HostPalette palette, string prefix, LogLevel maxLogLevel)
        {
            _p = palette;
            _writer = writer;
            _prefix = prefix;
            _maxLogLevel = maxLogLevel;
        }

        private string Prefix => $"{_p.Prefix}{_prefix}: {_p.Reset}";

        /// <summary>Writes the specified message.</summary>
        public void Write(string message, LogLevel logLevel)
        {
            var level = (int)logLevel;
            if (logLevel > _maxLogLevel ||
                string.IsNullOrEmpty(message))
            {
                return;
            }

            var offset = new string(' ', level * 2);
            var color = logLevel switch
            {
                LogLevel.Error => _p.Error,
                LogLevel.Message => _p.Message,
                LogLevel.Info => _p.Information,
                LogLevel.Verbose => _p.Debug,
                LogLevel.Debug => _p.Trace,
                _ => _p.None,
            };

            var msgs = CleanUp(message)
                .Split(Environment.NewLine)
                .Where(msg => !string.IsNullOrWhiteSpace(msg))
                .Select(msg => Message(color, offset + msg));

            _writer.Write(string.Join(Environment.NewLine, msgs));
        }

        /// <summary>Writes the specified message with new line.</summary>
        public void WriteLine(string message, LogLevel logLevel)
        {
            if (logLevel <= _maxLogLevel)
            {
                Write(message, logLevel);
                WriteLine();
            }
        }

        /// <summary>Writes a new line.</summary>
        public void WriteLine() =>
            _writer.WriteLine();

        private string Message(string color, string text) => string.Concat(Prefix, color, text, _p.Reset);

        private string CleanUp(string message) => message
            .Replace("\u001B[0m", string.Empty)
            .Replace("\u001B[32m", string.Empty)
            .Replace("\u001B[39m", string.Empty)
            .Replace("\u001B[94m", string.Empty)
            .Replace("\u001B[96m", string.Empty);
    }
}
