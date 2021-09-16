using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace DevOps.Packages
{
    /// <summary>JRE installer.</summary>
    [ExcludeFromCodeCoverage]
    public static class JavaVersionInstaller
    {
        /// <summary>Installs java on linux.</summary>
        public static void InstallLinux(string version)
        {
            UnixInstallJabba();

            var ls = Targets.Exec("jabba ls " + version, Targets.LogLevel.Debug, null);
            if (ls.Output.Contains(version, StringComparison.InvariantCultureIgnoreCase))
            {
                Targets.WriteLine($"JAVA {version} is installed.");
                Targets.Exec("jabba use " + version);
            }
            else
            {
                Targets.WriteLine($"Installing JAVA {version} on Linux.");
                Targets.Exec("jabba install " + version);
            }
        }

        /// <summary>Installs JRE on windows.</summary>
        public static void InstallWindow(string version)
        {
            throw new NotSupportedException("Windows installer is not supported " + version);
        }

        /// <summary>Installs the specified version.</summary>
        public static void Install(string version)
        {
            var test = Targets.Exec("java --version", Targets.LogLevel.Debug, null);
            if (test.ExitCode == 0)
            {
                Targets.WriteLine("Java exists with version " + test.Output);
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

        private static void UnixInstallJabba()
        {
            Targets.WriteLine("Check jabba is installed.");
            var testCommand = Targets.Exec("jabba --version", Targets.LogLevel.Debug, null);
            if (testCommand.ExitCode == 0)
            {
                Targets.WriteLine("jabba is installed.");
            }
            else
            {
                Targets.ExecRaw($"curl -sL https://github.com/shyiko/jabba/raw/master/install.sh | bash && . ~/.jabba/jabba.sh");
            }
        }
    }
}
