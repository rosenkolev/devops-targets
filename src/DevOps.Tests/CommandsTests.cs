using System;

using DevOps.Terminal.Terminals;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOps.Tests
{
    [TestClass]
    public class CommandsTests
    {
        [TestMethod]
        public void ShellShouldExec()
        {
            LoggerTests.InitNullLogger();
            var result = Targets.CommandExec("echo shell test");
            Assert.AreEqual("shell test", result.TextOutput);
        }

        [TestMethod]
        public void TerminalShouldExec()
        {
            LoggerTests.InitNullLogger();
            var result = Targets.Exec("echo shell test");
            Assert.AreEqual("shell test", result.Output);
        }

        [TestMethod]
        public void TerminalShouldPipeCommands()
        {
            LoggerTests.InitNullLogger();
            var command =
                TerminalCommand.Create("echo", "Test1") &
                TerminalCommand.Create("echo", "Test2");

            var result = TerminalSingleton.DefaultTerminal.Exec(command);
            Assert.AreEqual($"Test1{Environment.NewLine}Test2", result.Output);
        }

        [TestMethod]
        public void TerminalShouldPipeTreeCommands()
        {
            LoggerTests.InitNullLogger();
            var command =
                TerminalCommand.Create("echo", "Test1") &
                TerminalCommand.Create("echo", "Test2") &
                TerminalCommand.Create("echo", "Test3");

            var result = TerminalSingleton.DefaultTerminal.Exec(command);
            Assert.AreEqual($"Test1{Environment.NewLine}Test2{Environment.NewLine}Test3", result.Output);
        }
    }
}
