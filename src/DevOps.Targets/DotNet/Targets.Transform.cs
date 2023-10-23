using System.IO;
using System.Text.Json;
using System.Xml.Linq;
using System.Xml.XPath;

using DevOps.SystemJson;

namespace DevOps
{
    /// <summary>Targets main class.</summary>
    public static partial class Targets
    {
        /// <summary>Envs the value record.</summary>
        public record EnvValue(string Key, string Value);

        /// <summary>Transform test, json and xml files.</summary>
        public static class Transform
        {
            /// <summary>Merge two json files.</summary>
            public static string MergeJsonFiles(string originalJsonFileName, string newJsonFileName, MergeOptions options = null)
            {
                var originalContent = (MergeJsonObject)File.ReadAllText(originalJsonFileName);
                var newContent = (MergeJsonObject)File.ReadAllText(newJsonFileName);
                return originalContent.Merge(newContent, options);
            }

            /// <summary>Transforms the settings.json.</summary>
            public static void TransformSettingsJson(string pathToSettingsJson, string pathToTransformJson, MergeOptions options = null)
            {
                WriteLine("Transforming file " + pathToSettingsJson);
                var transformedContent = MergeJsonFiles(pathToSettingsJson, pathToTransformJson, options);
                WriteLine("Transformed file content " + transformedContent, LogLevel.Debug);

                File.WriteAllText(pathToSettingsJson, transformedContent);
            }

            /// <summary>Finds the property value in a json.</summary>
            public static string FindPropertyValueInJson(string pathToJson, string propertyName)
            {
                var sourceJsonContent = File.ReadAllText(pathToJson);
                using var jsonDocument = JsonDocument.Parse(sourceJsonContent);
                var jsonElement = jsonDocument.RootElement;
                var paths = propertyName.Split('.');
                foreach (var path in paths)
                {
                    if (!jsonElement.TryGetProperty(path, out jsonElement))
                    {
                        WriteLine($"Search of {propertyName} didn't find {path}!", LogLevel.Info);
                        return null;
                    }
                }

                WriteLine($"Property {propertyName} found", LogLevel.Info);
                return jsonElement.ToString();
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
        }
    }
}
