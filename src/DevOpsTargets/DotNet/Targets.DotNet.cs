using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace DevOpsTargets
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

        /// <summary>.NET tasks.</summary>
        public static class DotNet
        {
            /// <summary>Builds the specified project.</summary>
            public static void Build(string pathToProject, string configuration = "Debug") =>
                Shell($"dotnet build -c {configuration}", pathToProject);

            /// <summary>Publish the specified project.</summary>
            public static void Publish(string pathToProject, string pathToOutput, string configuration = "Release") =>
                Shell($"dotnet publish -c {configuration} -o {pathToOutput} -p:AnalysisLevel=none", pathToProject);

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

                var ls = string.Join(" ", loggers.Select(l => l.Build(pathToOutput)));
                var fs = string.Join(",", GetCoverageReportFormats(formats));
                Shell(
                    $"{GetTestCmd(configuration)}{ls} /p:CollectCoverage=true /p:CoverletOutputFormat=\\\"{fs}\\\" /p:CoverletOutput={pathToOutput}/",
                    pathToProject);
            }

            /// <summary>Test the specified project.</summary>
            public static void Test(string pathToProject, string configuration = "Debug", string pathToOutput = null) =>
                Shell(GetTestCmd(configuration) + DotNetLoggers.Trx.Build(pathToOutput), pathToProject);

            private static string GetTestCmd(string configuration) =>
                $"dotnet test -c {configuration} -p:AnalysisLevel=none ";
        }

        /// <summary>Tests logger info.</summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313", Justification = "record")]
        public record TestLogInfo(string Name, string PathProp, string FileName, string Params)
        {
            /// <summary>Builds the specified path to output.</summary>
            public string Build(string pathToOutput)
            {
                var output = pathToOutput == null ? FileName : Path.Combine(pathToOutput, FileName);
                return $"-l \"{Name};{PathProp}={output}{Params}\"";
            }
        }

        /// <summary>.NET loggers.</summary>
        public static class DotNetLoggers
        {
            /// <summary>The MSTest logger.</summary>
            public static readonly TestLogInfo Trx = new TestLogInfo("trx", "LogFileName", "dotnet.results.trx", string.Empty);

            /// <summary>The JUnit logger.</summary>
            public static readonly TestLogInfo JUnit = new TestLogInfo("junit", "LogFilePath", "dotnet.results.xml", ";MethodFormat=Class;FailureBodyFormat=Verbose");
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
    }
}
