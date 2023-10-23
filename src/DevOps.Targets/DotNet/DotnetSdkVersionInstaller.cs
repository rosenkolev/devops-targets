using System.Runtime.InteropServices;

namespace DevOps.DotNet
{
    /// <summary>.NET SDK and runtime installer.</summary>
    public static class DotnetSdkVersionInstaller
    {
        /// <summary>Checks the version is installed.</summary>
        public static bool CheckVersion(string version)
        {
            var ver = version.TrimStart('v', 'V');
            var res = Targets.Exec(" dotnet --list-sdks", Targets.LogLevel.Debug, null);
            return res.ExitCode == 0 && res.Output.Contains(ver);
        }

        /// <summary>Installs the specified version.</summary>
        public static void Install(string version)
        {
            Targets.WriteLine("Installing .NET SDK version " + version);
            if (CheckVersion(version))
            {
                Targets.WriteLine(".NET SDK version is already installed");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                InstallWindows();
            }
            else
            {
                InstallUnix(version);
            }
        }

        /// <summary>Installs on unix based OS.</summary>
        public static void InstallUnix(string version) =>
            Targets.Exec($"curl - sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin -c {version} --install-dir /usr/share/dotnet");

        /// <summary>Installs on windows based OS.</summary>
        public static void InstallWindows()
        {
            Targets.WriteLine("Install in windows is not supported");
        }
    }
}
