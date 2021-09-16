using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using static DevOps.Targets;

namespace DevOps.Tests
{
    [TestClass]
    public class TransformTests
    {
        [TestMethod]
        public void FindPropertyValueInJsonShouldFind()
        {
            LoggerTests.InitNullLogger();
            var fileName = CreateJsonFile("{\"first\":{\"second\":5}}");
            
            var data = Transform.FindPropertyValueInJson(fileName, "first.second");
            
            File.Delete(fileName);

            Assert.AreEqual("5", data);
        }

        [TestMethod]
        public void FindPropertyValueInJsonShouldNotFind()
        {
            LoggerTests.InitNullLogger();
            var fileName = CreateJsonFile("{\"first\":{\"second\":5}}");

            var data = Transform.FindPropertyValueInJson(fileName, "first.unknown");

            File.Delete(fileName);

            Assert.IsNull(data);
        }

        [TestMethod]
        public void TransformSettingsJsonShouldOverride()
        {
            LoggerTests.InitNullLogger();
            var fileName = CreateJsonFile("{\"first\":{\"second\":5,\"left\":1}}");
            var fileName2 = CreateJsonFile("{\"first\":{\"second\":6,\"right\":2}}");

            Transform.TransformSettingsJson(fileName, fileName2, new SystemJson.MergeOptions { Indented = false });

            var newContent = File.ReadAllText(fileName);

            File.Delete(fileName);
            File.Delete(fileName2);

            Assert.AreEqual("{\"first\":{\"second\":6,\"left\":1,\"right\":2}}", newContent);
        }

        [TestMethod]
        public void GetXmlXPathValueShouldFind()
        {
            LoggerTests.InitNullLogger();
            var fileName = CreateXmlFile("<root><first><second>500</second></first></root>");

            var data = Transform.GetXmlXPathValue(fileName, "/first/second");

            File.Delete(fileName);

            Assert.AreEqual("500", data);
        }

        private static string CreateTempFile(string content, string ext)
        {
            var filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ext);
            File.WriteAllText(filePath, content);
            return filePath;
        }

        private static string CreateJsonFile(string content) => CreateTempFile(content, ".json");
        private static string CreateXmlFile(string content) => CreateTempFile(content, ".xml");
    }
}
