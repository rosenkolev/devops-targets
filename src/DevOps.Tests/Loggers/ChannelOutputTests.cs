using System;

using DevOps.Terminal.Loggers;
using DevOps.Terminal.Loggers.Abstraction;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOps.Tests.Loggers
{
    [TestClass]
    public class ChannelOutputTests
    {
        [TestMethod]
        public void ChannelOutputShouldBeFiFo()
        {
            var channelOutput = new ChannelOutput();

            channelOutput.Write("Test Write", LogLevel.Info);
            channelOutput.WriteLine("Test WriteLine", LogLevel.Info);

            Assert.AreEqual("Test Write", channelOutput.WaitAndRead().Message);
            Assert.AreEqual("Test WriteLine" + Environment.NewLine, channelOutput.WaitAndRead().Message);
        }
    }
}
