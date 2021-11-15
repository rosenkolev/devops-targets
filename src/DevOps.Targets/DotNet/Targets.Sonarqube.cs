using System;
using System.IO;
using System.Linq;
using System.Text;

using DevOps.Terminal.Terminals;

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

            /// <summary>Run a sonarscanner begin to end.</summary>
            public static void RunScanner(
                Action buildAction,
                string workingDirectory,
                string key,
                string organization = null,
                string version = null,
                string branch = null)
            {
                var builder = new StringBuilder("dotnet sonarscanner begin /k:");
                builder.Append(key);
                if (organization != null)
                {
                    builder.Append(" /o:");
                    builder.Append(organization);
                }

                if (version != null)
                {
                    builder.Append(" /v:");
                    builder.Append(version);
                }

                if (branch != null)
                {
                    builder.Append(" /d:sonar.branch.name=");
                    builder.Append(branch);
                }

                Exec(
                    TerminalCommand.Cd(workingDirectory) &
                    TerminalCommand.CreateParse("dotnet sonarscanner begin " + builder, null));

                buildAction?.Invoke();

                Exec(TerminalCommand.CreateParse("dotnet sonarscanner end", null));
            }
        }
    }
}
