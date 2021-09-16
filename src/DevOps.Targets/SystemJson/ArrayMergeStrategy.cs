namespace DevOps.SystemJson
{
    /// <summary>Array merge strategy.</summary>
    public enum ArrayMergeStrategy
    {
        /// <summary>Replace original array.</summary>
        Replace = 1,

        /// <summary>Union the two array. Duplication is possible.</summary>
        Union = 2,
    }
}
