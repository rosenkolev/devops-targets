using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace DevOps.Packages
{
    /// <summary>NodeJs installer.</summary>
    [ExcludeFromCodeCoverage]
    public static class NodeJsVersionInstaller
    {
        private static readonly Regex VersionRegex = new(@"^\d+\.\d+\.\d+$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>Installs Node on linux.</summary>
        public static void InstallLinux(string version)
        {
            LinuxInstallNvm("v0.38.0");

            var ver = GetVersion(version);
            var ls = Targets.Exec("nvm ls " + ver, Targets.LogLevel.Debug, null);
            if (ls.ExitCode == 0)
            {
                Targets.WriteLine($"NodeJs {version}.x is installed.");
                Targets.Exec("nvm use " + ver);
            }
            else
            {
                Targets.WriteLine($"Installing NodeJs {version}.x on Linux.");
                Targets.Exec("nvm install " + ver);
            }
        }

        /// <summary>Installs Node on windows.</summary>
        public static void InstallWindow(string version)
        {
            var ver = GetVersion(version);
            var fileName = $"node-v{ver}-x86.msi";
            var pathFile = Downloader.DownloadFile("NodeJs", version, fileName, $"http://nodejs.org/dist/v{ver}/{fileName}");
            Targets.Exec("START /WAIT " + pathFile);
        }

        /// <summary>Installs the specified version.</summary>
        public static void Install(string version)
        {
            var test = Targets.Exec("node --version", Targets.LogLevel.Debug, null);
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

        private static void LinuxInstallNvm(string version)
        {
            Targets.WriteLine("Check NVM is installed.");
            var testCommand = Targets.Exec("nvm --version", Targets.LogLevel.Debug, null);
            if (testCommand.ExitCode == 0)
            {
                Targets.WriteLine("NVM is installed.");
            }
            else
            {
                var homeDir = Environment.GetEnvironmentVariable("HOME");
                var nvmPath = Path.Combine(homeDir, ".nvm");
                if (!Directory.Exists(nvmPath))
                {
                    Targets.ExecRaw($"wget -qO- https://raw.githubusercontent.com/nvm-sh/nvm/{version}/install.sh | bash");
                }
                else
                {
                    Targets.WriteLine("NVM install directory is found.");
                }

                Targets.ExecRaw(@"export NVM_DIR=""$HOME/.nvm""");
                Targets.ExecRaw(@"[ -s ""$NVM_DIR/nvm.sh"" ] && \. ""$NVM_DIR/nvm.sh""");
            }
        }

        private static string GetVersion(string version)
        {
            var ver = version.TrimStart('v', 'V');
            if (!VersionRegex.IsMatch(ver))
            {
                throw new ArgumentException("Version need to be in the format 16.1.2.", nameof(version));
            }

            return ver;
        }
    }
}
