using System;
using System.IO;
using System.Linq;

namespace DevOps
{
    /// <summary>Targets main class.</summary>
    public static partial class Targets
    {
        /// <summary>Sonarqube configure settings.</summary>
        public static class Sonarqube
        {
            /// <summary>Transforms the global SonarQube settings.</summary>
            public static void TransformGlobalSettings(string sonarScannerDirectory, string pathToTools, string sdk, params EnvValue[] values)
            {
                var pathsToSettings = Directory.GetFiles(sonarScannerDirectory, "SonarQube.Analysis.xml", SearchOption.AllDirectories);

                WriteLine($"Found {pathsToSettings.Length} SonarQube.Analysis.xml searching sdk {sdk}.", LogLevel.Info);

                var pathToMySettings = pathsToSettings.First(p => p.Contains(sdk));
                var pathToSonarqube = Path.Combine(pathToTools, "sonarqube.xml");
                var pathToTransformedSonarqube = Path.Combine(pathToTools, "SonarQube.Analysis.xml");

                if (!File.Exists(pathToMySettings))
                {
                    WriteLine($"File SonarQube.Analysis.xml was not found for sdk {sdk}.", LogLevel.Error);
                    return;
                }

                WriteLine("Modify tools/sonarqube.xml add base path.", LogLevel.Info);
                Transform.ReplaceInFile(pathToSonarqube, pathToTransformedSonarqube, values);

                WriteLine("Copy tools/sonarqube.xml to sonarqube root location " + pathToMySettings, LogLevel.Info);
                File.Copy(pathToTransformedSonarqube, pathToMySettings, true);
            }

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
