namespace DevOps.SystemJson
{
    /// <summary>Json merge options.</summary>
    public class MergeOptions
    {
        /// <summary>Gets or sets the array merge strategy.</summary>
        public ArrayMergeStrategy MergeArrays { get; set; } = ArrayMergeStrategy.Replace;

        /// <summary>Gets or sets a value indicating whether the output json is indented.</summary>
        public bool Indented { get; set; } = true;
    }
}
