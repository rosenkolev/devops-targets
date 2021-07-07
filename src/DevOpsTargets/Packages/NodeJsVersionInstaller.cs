using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace DevOpsTargets.Packages
{
    /// <summary>NodeJs installer.</summary>
    public static class NodeJsVersionInstaller
    {
        private static readonly Regex VersionRegex = new (@"^(\d+)\.\d+\.\d+$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>Installs Node on linux.</summary>
        public static void InstallLinux(string version = null)
        {
            var major = "lts";
            var ver = string.Empty;
            if (!string.IsNullOrEmpty(version))
            {
                (major, ver) = GetVersion(version);
            }

            Targets.WriteLine($"Installing NodeJs {major}.x on Linux.");
            Targets.Shell($"curl -fsSL https://deb.nodesource.com/setup_{major}.x | bash - && apt-get install -y nodejs=" + ver);
        }

        /// <summary>Installs Node on windows.</summary>
        public static void InstallWindow(string version)
        {
            var (_, ver) = GetVersion(version);
            var fileName = $"node-v{ver}-x86.msi";
            var pathFile = Downloader.DownloadFile("NodeJs", version, fileName, $"http://nodejs.org/dist/v{ver}/{fileName}");
            Targets.Shell("START /WAIT " + pathFile);
        }

        /// <summary>Installs the specified version.</summary>
        public static void Install(string version)
        {
            var test = Targets.ShellCommand("node --version", null, LogLevel.Debug);
            if (test.ExitCode == 0)
            {
                Targets.WriteLine("NodeJs exists with version " + test.Output);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                InstallWindow(version);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                InstallLinux(version);
            }
        }

        private static (string Major, string Ver) GetVersion(string version)
        {
            var ver = version.TrimStart('v', 'V');
            var match = VersionRegex.Match(ver);
            if (match == null || match.Groups.Count < 2)
            {
                throw new ArgumentException("Version need to be in the format 16.1.2.", nameof(version));
            }

            return (Major: match.Groups[1].Value, Ver: ver);
        }
    }
}
