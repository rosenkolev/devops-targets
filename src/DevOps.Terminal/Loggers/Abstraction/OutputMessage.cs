namespace DevOps.Terminal.Loggers.Abstraction
{
    /// <summary>An output message.</summary>
    public class OutputMessage
    {
        /// <summary>Gets the log level.</summary>
        public LogLevel Level { get; init; }

        /// <summary>Gets the message.</summary>
        public string Message { get; init; }

        /// <inheritdoc/>
        public override string ToString() => Message;
    }
}
