using DevOpsTargets.Packages;

namespace DevOpsTargets
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
        }
    }
}
