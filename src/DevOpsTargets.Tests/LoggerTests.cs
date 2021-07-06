using System.IO;

using Bullseye;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOpsTargets.Tests
{
    [TestClass]
    public class LoggerTests
    {
        [TestMethod]
        public void LoggerShouldWriteMessage()
        {
            using var stringWriter = new StringWriter();
            Targets.InitHostOutput(new Options(), string.Empty, stringWriter);

            Targets.Write("test");

            var text = stringWriter.ToString();
            Assert.IsTrue(text.Contains("test"));
        }

        public static void InitNullLogger() =>
            Targets.InitHostOutput(new Options(), string.Empty, TextWriter.Null);
    }
}
