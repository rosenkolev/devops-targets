﻿namespace DevOps.SystemJson
{
    /// <summary>Json object used in merging.</summary>
    public class MergeJsonObject
    {
        private readonly string _jsonContent;

        /// <summary>Initializes a new instance of the <see cref="MergeJsonObject"/> class.</summary>
        public MergeJsonObject(string jsonContent)
        {
            _jsonContent = jsonContent;
        }

        /// <summary>Convert merge object to string.</summary>
        public static implicit operator string(MergeJsonObject json) => json.ToString();

        /// <summary>Convert json content to merge object.</summary>
        public static explicit operator MergeJsonObject(string json) => new(json);

        /// <inheritdoc/>
        public override string ToString() => _jsonContent;
    }
}
