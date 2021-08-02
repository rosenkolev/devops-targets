namespace DevOps
{
    /// <summary>Targets main class.</summary>
    public static partial class Targets
    {
        /// <summary>The logger log levels.</summary>
        public enum LogLevel
        {
            /// <summary>Error.</summary>
            Error = 0,

            /// <summary>Message.</summary>
            Message,

            /// <summary>Info.</summary>
            Info,

            /// <summary>Verbose.</summary>
            Verbose,

            /// <summary>Debug.</summary>
            Debug,

            /// <summary>No logging.</summary>
            None,
        }
    }
}
