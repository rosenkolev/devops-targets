using System.IO;

using Bullseye;

using DevOps.Terminal;
using DevOps.Terminal.Loggers.Host;

using HostLogLevel = DevOps.Terminal.Loggers.Abstraction.LogLevel;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOps.Tests
{
    [TestClass]
    public class LoggerTests
    {
        [TestMethod]
        public void LoggerShouldWriteMessage()
        {
            using var stringWriter = new StringWriter();
            InitLogger(stringWriter);

            Targets.Write("test");

            var text = stringWriter.ToString();
            Assert.IsTrue(text.Contains("test"));
        }

        public static void InitLogger(TextWriter writer) =>
            Out.ReInitConsoleOutput(
                new HostOutput(writer, new HostPalette(), string.Empty, HostLogLevel.None));

        public static void InitNullLogger() => InitLogger(TextWriter.Null);
    }
}
