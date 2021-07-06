using System;
using System.IO;

using Bullseye.Internal;

namespace DevOpsTargets
{
    /// <summary>A host writer class.</summary>
    public sealed class HostOutput
    {
        private readonly string _prefix;
        private readonly TextWriter _writer;
        private readonly Palette p;

        /// <summary>Initializes a new instance of the <see cref="HostOutput"/> class.</summary>
        public HostOutput(TextWriter writer, Palette palette, string prefix)
        {
            p = palette;
            _writer = writer;
            _prefix = prefix;
        }

        /// <summary>Gets or sets the maximum log level.</summary>
        public static LogLevel MaxLogLevel { get; set; } = LogLevel.Verbose;

        /// <summary>Writes the specified message.</summary>
        public void Write(string message, LogLevel logLevel)
        {
            var level = (int)logLevel;
            if (MaxLogLevel < logLevel)
            {
                return;
            }

            var offset = new string(' ', level * 2);
            var msg = offset + CleanUp(message).Replace(Environment.NewLine, Environment.NewLine + offset);
            switch (level)
            {
                case 0: this._writer.Write(Message(p.Failed, msg)); break;  // Error
                case 1: this._writer.Write(Message(p.Default, msg)); break; // Message
                case 2: this._writer.Write(Message(p.Invocation, msg)); break;  // Info
                case 3: this._writer.Write(Message(p.Input, msg)); break; // Verbose
                case 4: this._writer.Write(Message(p.Option, msg)); break; // Debug
                default: break;
            }
        }

        /// <summary>Writes the specified message with new line.</summary>
        public void WriteLine(string message, LogLevel logLevel)
        {
            Write(message, logLevel);
            _writer.WriteLine();
        }

        private string Message(string color, string text) => $"{p.Prefix}{_prefix}: {p.Reset}{color}{text}{p.Reset}";

        private string CleanUp(string message) => message
            .Replace("[0m", string.Empty)
            .Replace("[32m", string.Empty)
            .Replace("[39m", string.Empty)
            .Replace("[94m", string.Empty)
            .Replace("[96m", string.Empty);
    }
}
