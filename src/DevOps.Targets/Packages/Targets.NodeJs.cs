using System;

using DevOps.Packages;

namespace DevOps
{
    /// <summary>Targets main class.</summary>
    public static partial class Targets
    {
        /// <summary>NodeJs tasks.</summary>
        public static class NodeJs
        {
            /// <summary>Installs the specified NodeJs version is not NodeJs version is installed.</summary>
            public static void Install(string version) =>
                NodeJsVersionInstaller.Install(version);

            /// <summary>Sets the NODE_PATH env var.</summary>
            public static void SetNodePath()
            {
                var pathToGlobalNodeModules = Exec("npm root -g");

                WriteLine("Set env variable NODE_PATH to " + pathToGlobalNodeModules.Output, LogLevel.Info);
                Environment.SetEnvironmentVariable("NODE_PATH", pathToGlobalNodeModules.Output);
            }
        }
    }
}
