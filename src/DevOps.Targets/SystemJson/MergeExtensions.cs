using System;
using System.Buffers;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace DevOps.SystemJson
{
    /// <summary>A json merge extensions.</summary>
    public static class MergeExtensions
    {
        /// <summary>Merge two json objects using the merge options.</summary>
        public static MergeJsonObject Merge(this MergeJsonObject originalJson, MergeJsonObject newContent, MergeOptions options = null)
        {
            options ??= new MergeOptions();

            var outputBuffer = new ArrayBufferWriter<byte>();

            using (JsonDocument jDoc1 = JsonDocument.Parse(originalJson))
            using (JsonDocument jDoc2 = JsonDocument.Parse(newContent))
            using (var jsonWriter = new Utf8JsonWriter(outputBuffer, new JsonWriterOptions { Indented = options.Indented }))
            {
                JsonElement root1 = jDoc1.RootElement;
                JsonElement root2 = jDoc2.RootElement;

                if (root1.ValueKind != JsonValueKind.Array && root1.ValueKind != JsonValueKind.Object)
                {
                    throw new InvalidOperationException($"The original JSON document must be a container type. Instead it is {root1.ValueKind}.");
                }

                if (root1.ValueKind != root2.ValueKind)
                {
                    return originalJson;
                }

                if (root1.ValueKind == JsonValueKind.Array)
                {
                    MergeArrays(jsonWriter, root1, root2, options);
                }
                else
                {
                    MergeObjects(jsonWriter, root1, root2, options);
                }
            }

            return (MergeJsonObject)Encoding.UTF8.GetString(outputBuffer.WrittenSpan);
        }

        private static void MergeObjects(Utf8JsonWriter jsonWriter, JsonElement root1, JsonElement root2, MergeOptions options)
        {
            Debug.Assert(root1.ValueKind == JsonValueKind.Object, "first root should be object");
            Debug.Assert(root2.ValueKind == JsonValueKind.Object, "second root should be object");

            jsonWriter.WriteStartObject();

            // Write all the properties of the first document.
            // If a property exists in both documents, either:
            // * Merge them, if the value kinds match (e.g. both are objects or arrays),
            // * Completely override the value of the first with the one from the second, if the value kind mismatches (e.g. one is object, while the other is
            // * an array or string), Or favor the value of the first (regardless of what it may be), if the second one is null (i.e. don't override the first).
            foreach (JsonProperty property in root1.EnumerateObject())
            {
                string propertyName = property.Name;

                JsonValueKind newValueKind;

                if (root2.TryGetProperty(propertyName, out JsonElement newValue) && (newValueKind = newValue.ValueKind) != JsonValueKind.Null)
                {
                    jsonWriter.WritePropertyName(propertyName);

                    JsonElement originalValue = property.Value;
                    JsonValueKind originalValueKind = originalValue.ValueKind;

                    if (newValueKind == JsonValueKind.Object && originalValueKind == JsonValueKind.Object)
                    {
                        MergeObjects(jsonWriter, originalValue, newValue, options); // Recursive call
                    }
                    else if (newValueKind == JsonValueKind.Array && originalValueKind == JsonValueKind.Array)
                    {
                        MergeArrays(jsonWriter, originalValue, newValue, options);
                    }
                    else
                    {
                        newValue.WriteTo(jsonWriter);
                    }
                }
                else
                {
                    property.WriteTo(jsonWriter);
                }
            }

            // Write all the properties of the second document that are unique to it.
            foreach (JsonProperty property in root2.EnumerateObject())
            {
                if (!root1.TryGetProperty(property.Name, out _))
                {
                    property.WriteTo(jsonWriter);
                }
            }

            jsonWriter.WriteEndObject();
        }

        private static void MergeArrays(Utf8JsonWriter jsonWriter, JsonElement root1, JsonElement root2, MergeOptions options)
        {
            Debug.Assert(root1.ValueKind == JsonValueKind.Array, "first root should be array");
            Debug.Assert(root2.ValueKind == JsonValueKind.Array, "second root should be array");

            jsonWriter.WriteStartArray();

            if (options.MergeArrays != ArrayMergeStrategy.Replace)
            {
                foreach (JsonElement element in root1.EnumerateArray())
                {
                    element.WriteTo(jsonWriter);
                }
            }

            foreach (JsonElement element in root2.EnumerateArray())
            {
                element.WriteTo(jsonWriter);
            }

            jsonWriter.WriteEndArray();
        }
    }
}
