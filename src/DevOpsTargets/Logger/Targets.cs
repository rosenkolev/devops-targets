using System.IO;
using System.Runtime.InteropServices;

using Bullseye;
using Bullseye.Internal;

using OperatingSystem = Bullseye.Internal.OperatingSystem;

namespace DevOpsTargets
{
    /// <summary>Targets main class.</summary>
    public static partial class Targets
    {
        /// <summary>Gets the console output.</summary>
        public static HostOutput ConsoleOutput { get; private set; }

        /// <summary>Writes a message to the output on a new line.</summary>
        public static void WriteLine(string message, LogLevel logLevel = LogLevel.Message) =>
            ConsoleOutput.WriteLine(message, logLevel);

        /// <summary>Writes a message to the output.</summary>
        public static void Write(string message, LogLevel logLevel = LogLevel.Message) =>
            ConsoleOutput.Write(message, logLevel);

        /// <summary>Initializes the host output.</summary>
        public static void InitHostOutput(Options options, string prefix, TextWriter hostWriter)
        {
            #pragma warning disable S3358

            var operatingSystem =
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? OperatingSystem.Windows
                    : RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                        ? OperatingSystem.Linux
                        : RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                            ? OperatingSystem.MacOS
                            : OperatingSystem.Unknown;

            #pragma warning restore S3358

            var noColor = options.NoColor;
            var host = options.Host.DetectIfUnknown().Item1;
            var palette = new Palette(noColor, options.NoExtendedChars, host, operatingSystem);
            if (options.Verbose)
            {
                HostOutput.MaxLogLevel = LogLevel.Debug;
            }

            ConsoleOutput = new HostOutput(hostWriter, palette, prefix);
        }
    }
}
