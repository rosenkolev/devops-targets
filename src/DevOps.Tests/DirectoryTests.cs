using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOps.Tests
{
    [TestClass]
    public class DirectoryTests
    {
        [TestMethod]
        public void EnsureDirectoryShouldCreateIt()
        {
            var temp = Path.Combine(Path.GetTempPath(), "TestTemp");
            Assert.IsFalse(Directory.Exists(temp));

            Targets.EnsureDirectoryExists(temp);

            Assert.IsTrue(Directory.Exists(temp));
            
            Directory.Delete(temp, true);
        }

        [TestMethod]
        public void DeleteAllShouldDelete()
        {
            var testFolder = Path.Combine(Path.GetTempPath(), "ForCleanFolder");
            var testFile = Path.Combine(Path.GetTempPath(), "ForCleanText.txt");
            Directory.CreateDirectory(testFolder);
            File.WriteAllText(testFile, ".");

            Targets.DeleteAllFilesAndFolders(testFolder, testFile);

            Assert.IsFalse(Directory.Exists(testFolder));
            Assert.IsFalse(Directory.Exists(testFile));
        }
    }
}
