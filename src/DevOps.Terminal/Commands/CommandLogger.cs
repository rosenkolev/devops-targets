using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using DevOps.Terminal.Loggers.Abstraction;

namespace DevOps.Terminal.Commands
{
    /// <summary>The default command logger.</summary>
    public class CommandLogger : ICommandLogger
    {
        /// <summary>Initializes a new instance of the <see cref="CommandLogger"/> class.</summary>
        public CommandLogger(LogLevel logLevel, params IOutput[] outputs)
        {
            LogLevel = logLevel;
            Outputs = new List<IOutput>(outputs);
        }

        /// <summary>Gets or sets the log level.</summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>Gets the logger outputs.</summary>
        internal List<IOutput> Outputs { get; init; }

        /// <summary>Logs the output.</summary>
        [SuppressMessage("Redundancies in Symbol Declarations", "RECS0154", Justification = "Based on DataReceived event")]
        public void LogOutput(object sender, DataReceivedEventArgs e) =>
            Outputs.ForEach(l => l.WriteLine(e.Data, LogLevel));

        /// <summary>Logs the error.</summary>
        [SuppressMessage("Redundancies in Symbol Declarations", "RECS0154", Justification = "Based on DataReceived event")]
        public void LogError(object sender, DataReceivedEventArgs e) =>
            Outputs.ForEach(l => l.WriteLine(e.Data, LogLevel.Error));

        /// <inheritdoc/>
        public void Add(IOutput output) =>
            Outputs.Add(output);
    }
}
