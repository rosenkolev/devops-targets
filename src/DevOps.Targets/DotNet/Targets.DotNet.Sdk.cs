using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using DevOps.Terminal.Commands;

namespace DevOps
{
    /// <summary>Targets main class.</summary>
    public static partial class Targets
    {
        /// <summary>.NET tasks.</summary>
        public static partial class DotNet
        {
            /// <summary>.NET SDK tasks.</summary>
            public static class Sdk
            {
                /// <summary>Install the specified version.</summary>
                public static void Install(string version) =>
                    DotNetSdkVersionInstaller.Install(version);

                /// <summary>Set the MSBuildSDKsPath variable used in msbuild.</summary>
                public static void SetMsBuildSdksPath()
                {
                    var result = Exec("dotnet --version", LogLevel.Debug, validExitCode: null);

                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        var windowsPath = $"C:\\Program Files\\dotnet\\sdk\\{result.Output}\\Sdks";
                        Exec($"setx MSBuildSDKsPath \"{windowsPath}\"");
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        var unixPath = $"/usr/share/dotnet/sdk/{result.Output}/Sdks";
                        Exec("export MSBuildSDKsPath=" + unixPath);
                    }
                }
            }
        }

        /// <summary>.NET tools management.</summary>
        public static class Tool
        {
            /// <summary>Install a .NET tool.</summary>
            /// <param name="name">The tool name.</param>
            /// <param name="version">An optional version.</param>
            /// <param name="global">Is it a global tool.</param>
            public static CommandResult Install(string name, string version = null, bool global = true) =>
                Install(name, global ? "-g" : null, string.IsNullOrEmpty(version) ? null : ("--version " + version));

            /// <summary>Install a .NET tool with arguments.</summary>
            internal static CommandResult Install(string name, params string[] args) =>
                Exec(string.Concat("dotnet tool install ", name, string.Join(' ', args.Where(arg => arg != null))), validExitCode: null);
        }

        /// <summary>JRE installer.</summary>
        [ExcludeFromCodeCoverage]
        internal static class DotNetSdkVersionInstaller
        {
            [SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded", Justification = "No External Settings")]
            private static readonly string DotNetScriptUrl = "https://dot.net/v1/{0}";

            /// <summary>Installs the specified version.</summary>
            public static void Install(string version)
            {
                var test = Exec("dotnet --list-sdks", LogLevel.Debug, null);
                if (test.ExitCode == 0 && test.Output.Contains(version, StringComparison.InvariantCultureIgnoreCase))
                {
                    WriteLine($".NET SDK version {version} is installed!");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    WriteLine($"Installing .NET SDK {version} on Windows.");
                    var windowsPath = DownloadInstallScript("dotnet-install.ps1");
                    Exec($"powershell -NoProfile -ExecutionPolicy unrestricted -File {windowsPath} -c {version}");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    WriteLine($"Installing .NET SDK {version} on Linux.");
                    var unixPath = DownloadInstallScript("dotnet-install.sh");
                    Exec($"bash {unixPath} -c {version} --install-dir /usr/share/dotnet");
                }
            }

            /// <summary>Download the install SDK script.</summary>
            internal static string DownloadInstallScript(string scriptName)
            {
                var path = Packages.Downloader.GetToolPath("dotnet-installer", "1.0", scriptName);
                if (!File.Exists(path))
                {
                    var url = string.Format(CultureInfo.InvariantCulture, DotNetScriptUrl, scriptName);
                    Packages.Downloader.DownloadFile("dotnet-installer", "1.0", scriptName, url);
                }

                return path;
            }
        }
    }
}
