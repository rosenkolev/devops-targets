using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using DevOps.DotNet;
using DevOps.Terminal.Terminals;

namespace DevOps
{
    /// <summary>Targets main class.</summary>
    public static partial class Targets
    {
        /// <summary>Test coverage formats.</summary>
        [Flags]
        public enum TestCoverageFormat
        {
            /// <summary>The cobertura.</summary>
            Cobertura = 1,

            /// <summary>The open cover.</summary>
            OpenCover = 2,
        }

        private static IEnumerable<string> GetCoverageReportFormats(TestCoverageFormat formats)
        {
            if ((formats & TestCoverageFormat.Cobertura) == TestCoverageFormat.Cobertura)
            {
                yield return "cobertura";
            }

            if ((formats & TestCoverageFormat.OpenCover) == TestCoverageFormat.OpenCover)
            {
                yield return "opencover";
            }
        }

        /// <summary>.NET tasks.</summary>
        public static partial class DotNet
        {
            /// <summary>Builds the specified project.</summary>
            public static void Build(string pathToProject, string configuration = "Debug") =>
                Exec($"dotnet build -c {configuration}", pathToProject);

            /// <summary>Publish the specified project.</summary>
            public static void Publish(string pathToProject, string pathToOutput, string configuration = "Release") =>
                Exec($"dotnet publish -c {configuration} -o {pathToOutput} -p:AnalysisLevel=none", pathToProject);

            /// <summary>Test the specified project with coverage.</summary>
            public static void TestWithCoverage(
                string pathToProject,
                string pathToOutput,
                string configuration = "Debug",
                TestCoverageFormat formats = TestCoverageFormat.Cobertura,
                params TestLogInfo[] loggers)
            {
                if (loggers == null || !loggers.Any())
                {
                    loggers = new[] { DotNetLoggers.Trx };
                }

                var fs = string.Join(",", GetCoverageReportFormats(formats));
                var args = new List<string>(
                    new[]
                    {
                        "dotnet",
                        "test",
                        "-c",
                        configuration,
                        "-p:AnalysisLevel=none",
                        "/p:CollectCoverage=true",
                        $"/p:CoverletOutputFormat=\"{fs}\"",
                        $"/p:CoverletOutput={pathToOutput}",
                    });

                foreach (var logger in loggers)
                {
                    args.Add("-l");
                    args.Add(logger.Build(pathToOutput));
                }

                Exec(
                    TerminalCommand.Cd(pathToProject) &
                    TerminalCommand.Create(args.ToArray()));
            }

            /// <summary>Test the specified project.</summary>
            public static void Test(string pathToProject, string configuration = "Debug", string pathToOutput = null) =>
                Exec(
                    TerminalCommand.Cd(pathToProject) &
                    TerminalCommand.Create(
                        "dotnet",
                        "test",
                        "-c",
                        configuration,
                        "-p:AnalysisLevel=none",
                        "-l",
                        DotNetLoggers.Trx.Build(pathToOutput)));

            /// <summary>Installs the SDK version.</summary>
            public static void InstallSdk(string version) =>
                DotnetSdkVersionInstaller.Install(version);
        }

        /// <summary>Tests logger info.</summary>
        public record TestLogInfo(string Name, string PathProp, string FileName, string Params)
        {
            /// <summary>Builds the specified path to output.</summary>
            public string Build(string pathToOutput)
            {
                var output = pathToOutput == null ? FileName : Path.Combine(pathToOutput, FileName);
                return $"\"{Name};{PathProp}={output}{Params}\"";
            }
        }

        /// <summary>.NET loggers.</summary>
        public static class DotNetLoggers
        {
            /// <summary>The MSTest logger.</summary>
            public static readonly TestLogInfo Trx = new("trx", "LogFileName", "dotnet.results.trx", string.Empty);

            /// <summary>The JUnit logger.</summary>
            public static readonly TestLogInfo JUnit = new("junit", "LogFilePath", "dotnet.results.xml", ";MethodFormat=Class;FailureBodyFormat=Verbose");
        }
    }
}
