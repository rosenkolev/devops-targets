using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
            public static void TransformGlobalSettings(string sonarScannerDirectory, string pathToSonarqube, string[] sdks, params EnvValue[] values)
            {
                const string sonarQubeSettings = "SonarQube.Analysis.xml";
                var pathsToSettings = Directory.GetFiles(sonarScannerDirectory, sonarQubeSettings, SearchOption.AllDirectories);

                WriteLine($"Found {pathsToSettings.Length} SonarQube.Analysis.xml in directory {sonarScannerDirectory}.", LogLevel.Info);

                var pathToMySettings = pathsToSettings.FirstOrDefault(p => sdks.Any(sdk => p.Contains(sdk)));
                var pathToTools = Path.GetDirectoryName(pathToSonarqube);
                var pathToTransformedSonarqube = Path.Combine(pathToTools, sonarQubeSettings);

                if (pathToMySettings == null || !File.Exists(pathToMySettings))
                {
                    var sdkValue = string.Join(",", sdks);
                    throw new InvalidOperationException($"File SonarQube.Analysis.xml was not found for sdk {sdkValue}.");
                }

                WriteLine("Modify tools/sonarqube.xml add base path.", LogLevel.Info);
                Transform.ReplaceInFile(pathToSonarqube, pathToTransformedSonarqube, values);

                WriteLine("Copy tools/sonarqube.xml to sonarqube root location " + pathToMySettings, LogLevel.Info);
                File.Copy(pathToTransformedSonarqube, pathToMySettings, true);
            }

            /// <summary>Transforms the global SonarQube settings.</summary>
            public static void TransformGlobalSettings(string sonarScannerDirectory, string pathToTools, string sdk, params EnvValue[] values) =>
                TransformGlobalSettings(sonarScannerDirectory, Path.Combine(pathToTools, "sonarqube.xml"), new[] { sdk }, values);

            /// <summary>Transforms the global SonarQube settings.</summary>
            public static void TransformGlobalSettings(string pathToSonarqube, string[] sdks, params EnvValue[] values) =>
                TransformGlobalSettings(
                    DotNet.Tool.GetGlobalToolStorePath("dotnet-sonarscanner"),
                    pathToSonarqube,
                    sdks,
                    values);

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

                Exec(TerminalCommand.Cd(workingDirectory) & TerminalCommand.CreateParse(builder.ToString(), null));

                buildAction?.Invoke();

                Exec(TerminalCommand.CreateParse("dotnet sonarscanner end", null));
            }
        }
    }
}
