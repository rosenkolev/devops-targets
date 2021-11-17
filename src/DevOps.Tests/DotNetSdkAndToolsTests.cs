using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOps.Tests
{
    [TestClass]
    public class DotNetSdkAndToolsTests
    {
        [TestMethod]
        public void ToolInstallAndUninstallShouldWorkGlobal()
        {
            var pathToFolder = Targets.DotNet.Tool.GetGlobalToolStorePath("dotnetsay");

            Assert.IsFalse(Directory.Exists(pathToFolder));
            
            Targets.DotNet.Tool.Install("dotnetsay", "2.1.0");

            Assert.IsTrue(Directory.Exists(pathToFolder));

            Targets.DotNet.Tool.Uninstall("dotnetsay");

            Assert.IsFalse(Directory.Exists(pathToFolder));
        }
    }
}
