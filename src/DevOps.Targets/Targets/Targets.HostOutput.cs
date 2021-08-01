using DevOps.Terminal;

using HostLogLevel = DevOps.Terminal.Loggers.Abstraction.LogLevel;

namespace DevOps
{
    /// <summary>Targets main class.</summary>
    public static partial class Targets
    {
        /// <summary>Writes a message to the output on a new line.</summary>
        public static void WriteLine(string message, LogLevel logLevel = LogLevel.Message) =>
            Out.WriteLine(message, (HostLogLevel)logLevel);

        /// <summary>Writes a message to the output.</summary>
        public static void Write(string message, LogLevel logLevel = LogLevel.Message) =>
            Out.Write(message, (HostLogLevel)logLevel);
    }
}
