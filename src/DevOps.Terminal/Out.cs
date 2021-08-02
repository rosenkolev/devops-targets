using System;

using DevOps.Terminal.Loggers.Abstraction;
using DevOps.Terminal.Loggers.Host;

namespace DevOps.Terminal
{
    /// <summary>The host output generic methods.</summary>
    public static class Out
    {
        private static HostOutput _consoleOutput;

        /// <summary>Gets the maximum log level.</summary>
        public static LogLevel MaxLogLevel { get; private set; } = LogLevel.Verbose;

        /// <summary>Gets the prefix.</summary>
        public static string Prefix { get; private set; }

        /// <summary>Gets the console output.</summary>
        public static HostOutput ConsoleOutput => _consoleOutput ?? ReInitConsoleOutput(LogLevel.Verbose, string.Empty);

        /// <summary>Writes a message to the output on a new line.</summary>
        public static void WriteLine(string message, LogLevel logLevel = LogLevel.Message) =>
            ConsoleOutput.WriteLine(message, logLevel);

        /// <summary>Writes a message to the output.</summary>
        public static void Write(string message, LogLevel logLevel = LogLevel.Message) =>
            ConsoleOutput.Write(message, logLevel);

        /// <summary>Creates the console output.</summary>
        public static HostOutput CreateConsoleOutput() =>
            new HostOutput(Console.Out, new HostPalette(), Prefix, MaxLogLevel);

        /// <summary>Re-initialize console output.</summary>
        public static HostOutput ReInitConsoleOutput(LogLevel logLevel, string prefix)
        {
            Prefix = prefix;
            MaxLogLevel = logLevel;
            return ReInitConsoleOutput(CreateConsoleOutput());
        }

        /// <summary>Re-initialize console output.</summary>
        public static HostOutput ReInitConsoleOutput(HostOutput consoleOutput)
        {
            _consoleOutput = consoleOutput;
            return _consoleOutput;
        }
    }
}
