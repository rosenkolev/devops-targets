using DevOps.Terminal.Commands;
using DevOps.Terminal.Loggers;
using DevOps.Terminal.Loggers.Abstraction;
using DevOps.Terminal.Loggers.Host;
using DevOps.Terminal.Terminals;
using DevOps.Tests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using TerminalIn = DevOps.Terminal.Terminals.Terminal;

namespace DevOps.Tests.TerminalIntegration
{
    [TestClass]
    public class TerminalIntegrationTests
    {
        [TestMethod]
        public void TestTerminalLoggerShouldLogTheSame()
        {
            LoggerTests.InitNullLogger();
            var logger = new CommandLogger(LogLevel.Message);
            var formatter = new HostOutputFormatter(
                new HostPalette(),
                prefix: string.Empty,
                offsetRation: 0,
                noColor: true);

            using var writter = MemoryTextStream.Create();
            var hostOutput = new HostOutput(writter.Writer, LogLevel.Message, formatter);
            var textOutput = new TextOutput();
            var monitor = new TerminalMonitor(logger, textOutput, hostOutput);
            var syntax = TerminalIn.DefaultTerminalSyntax;
            var terminal = new TerminalIn(syntax, monitor, TerminalIn.CreateCommand(syntax, null, logger));

            var result = terminal.Exec(
                TerminalCommand.Create("echo", "Test1") &
                TerminalCommand.Create("echo", "Test2") &
                TerminalCommand.Create("echo", "Test3"));

            var value = writter.GetText().TrimEnd('\r', '\n');

            Assert.AreEqual(value, result.Output);
        }
    }
}
