using DevOps.SystemJson;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOps.Tests
{
    [TestClass]
    public class SystemJsonTests
    {
        [TestMethod]
        public void MergeTwoJsonWithArrayUnion()
        {
            var firstJson = new MergeJsonObject("{ \"num\": 1, \"str\": \"2\", \"leftProp\": \"3\", \"arr\": [10, 11] }");
            var secondJson = new MergeJsonObject("{ \"num\": 100, \"str\": \"101\", \"rightProp\": 4, \"arr\": [12, 13] }");
            var expected = firstJson.Merge(secondJson, new MergeOptions { MergeArrays = ArrayMergeStrategy.Union, Indented = false });
            Assert.AreEqual("{\"num\":100,\"str\":\"101\",\"leftProp\":\"3\",\"arr\":[10,11,12,13],\"rightProp\":4}", expected.ToString());
        }

        [TestMethod]
        public void MergeTwoJsonWithArrayReplace()
        {
            var firstJson = new MergeJsonObject("{ \"num\": 1, \"str\": \"2\", \"arr\": [10, 11] }");
            var secondJson = new MergeJsonObject("{ \"arr\": [12, 13] }");
            var expected = firstJson.Merge(secondJson, new MergeOptions { MergeArrays = ArrayMergeStrategy.Replace, Indented = false });
            Assert.AreEqual("{\"num\":1,\"str\":\"2\",\"arr\":[12,13]}", expected.ToString());
        }
    }
}
