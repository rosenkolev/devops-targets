using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;

using Newtonsoft.Json.Linq;

namespace DevOps
{
    /// <summary>Targets main class.</summary>
    public static partial class Targets
    {
        /// <summary>Envs the value record.</summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313", Justification = "records")]
        public record EnvValue(string Key, string Value);

        /// <summary>Transform test, json and xml files.</summary>
        public static class Transform
        {
            /// <summary>Transforms the settings.json.</summary>
            public static void TransformSettingsJson(string pathToSettingsJson, string pathToTransformJson)
            {
                WriteLine("Transforming file " + pathToSettingsJson);
                var sourceJson = ReadJsonFile(pathToSettingsJson);
                var transformJson = ReadJsonFile(pathToTransformJson);

                sourceJson.Merge(transformJson);

                var transformedContent = sourceJson.ToString();
                WriteLine("Transformed file content " + transformedContent, LogLevel.Debug);

                File.WriteAllText(pathToSettingsJson, transformedContent);
            }

            /// <summary>Finds the property value in a json.</summary>
            public static string FindPropertyValueInJson(string pathToJson, string propertyName)
            {
                var sourceJson = ReadJsonFile(pathToJson) as JToken;
                var paths = propertyName.Split('.');
                foreach (var path in paths)
                {
                    if (sourceJson == null)
                    {
                        return null;
                    }

                    sourceJson = sourceJson[path];
                }

                return (string)sourceJson;
            }

            /// <summary>Gets the XML xpath value.</summary>
            public static string GetXmlXPathValue(string pathToXml, string xpath)
            {
                WriteLine($"Get xpath {xpath} from {pathToXml}.", LogLevel.Info);
                var xml = XElement.Load(pathToXml);
                return xml.XPathSelectElement(xpath)?.Value;
            }

            /// <summary>Replaces strings in file.</summary>
            public static void ReplaceInFile(string pathToFile, string pathToNewFile, params EnvValue[] values)
            {
                var content = File.ReadAllText(pathToFile);
                string transformedContent = content;
                foreach (var env in values)
                {
                    transformedContent = transformedContent.Replace(env.Key, env.Value);
                }

                File.WriteAllText(pathToNewFile ?? pathToFile, transformedContent);
            }

            private static JObject ReadJsonFile(string pathToSettingsJson) =>
                JObject.Parse(File.ReadAllText(pathToSettingsJson));
        }
    }
}
