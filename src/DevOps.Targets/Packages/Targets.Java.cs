using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace DevOps
{
    /// <summary>Targets main class.</summary>
    public static partial class Targets
    {
        /// <summary>Java related tasts.</summary>
        public static class Java
        {
            /// <summary>Install the specified version.</summary>
            public static void Install(string version) =>
                JavaVersionInstaller.Install(version);
        }

        /// <summary>JRE version installer.</summary>
        [ExcludeFromCodeCoverage]
        internal static class JavaVersionInstaller
        {
            /// <summary>Installs java on linux.</summary>
            public static void InstallLinux(string version)
            {
                UnixInstallJabba();

                var ls = Exec("jabba ls " + version, Targets.LogLevel.Debug, null);
                if (ls.Output.Contains(version, StringComparison.InvariantCultureIgnoreCase))
                {
                    WriteLine($"JAVA {version} is installed.");
                    Exec("jabba use " + version);
                }
                else
                {
                    WriteLine($"Installing JAVA {version} on Linux.");
                    Exec("jabba install " + version);
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
                var test = Exec("java --version", Targets.LogLevel.Debug, null);
                if (test.ExitCode == 0)
                {
                    WriteLine("Java exists with version " + test.Output);
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
                WriteLine("Check jabba is installed.");
                var testCommand = Exec("jabba --version", LogLevel.Debug, null);
                if (testCommand.ExitCode == 0)
                {
                    WriteLine("jabba is installed.");
                }
                else
                {
                    ExecRaw($"curl -sL https://github.com/shyiko/jabba/raw/master/install.sh | bash && . ~/.jabba/jabba.sh");
                }
            }
        }
    }
}
