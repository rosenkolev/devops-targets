using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOpsTargets.Tests
{
    [TestClass]
    public class CommandsTests
    {
        [TestMethod]
        public void ShellShouldExec()
        {
            LoggerTests.InitNullLogger();
            var result = Targets.Shell("echo shell test");
            Assert.AreEqual("shell test", result);
        }
    }
}
