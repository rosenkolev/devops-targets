﻿namespace DevOpsTargets
{
    /// <summary>Targets main class.</summary>
    public static partial class Targets
    {
        /// <summary>NPM tasks.</summary>
        public static class Npm
        {
            /// <summary>Installs all packages from package.json file.</summary>
            public static void Install(string pathToProjectOrSolutionFolder) =>
                Shell("npm install", pathToProjectOrSolutionFolder);

            /// <summary>Installs all packages from package.json file.</summary>
            public static void Ci(string pathToProjectOrSolutionFolder, string cacheFolder = null)
            {
                var cache = string.IsNullOrEmpty(cacheFolder) ? string.Empty : ("--cache " + cacheFolder);
                Shell("npm ci --no-optional --prefer-offline " + cache, pathToProjectOrSolutionFolder);
            }

            /// <summary>Run an NPM command.</summary>
            public static void Run(string cmd, string pathToFolder) =>
                Shell("npm run " + cmd, pathToFolder);

            /// <summary>Installs the global NPM package.</summary>
            public static void InstallGlobal(string command)
            {
                WriteLine($"Checking {command} exist!", LogLevel.Debug);
                var result = ShellCommand($"npm ls -g --depth=0 {command}", null, LogLevel.Debug);
                if (!result.Output.Contains($"-- {command}"))
                {
                    WriteLine($"Installing NPM package {command}!", LogLevel.Info);
                    Shell($"npm install {command} -g");
                }
                else
                {
                    WriteLine($"Already installed NPM package {command}!", LogLevel.Info);
                }
            }
        }
    }
}
