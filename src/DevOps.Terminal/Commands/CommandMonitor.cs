using System;
using System.Linq;

using DevOps.Terminal.Loggers;
using DevOps.Terminal.Loggers.Abstraction;

namespace DevOps.Terminal.Commands
{
    /// <summary>A command output monitor.</summary>
    public class CommandMonitor
    {
        private readonly ChannelOutput _channelOutput;

        /// <summary>Initializes a new instance of the <see cref="CommandMonitor"/> class.</summary>
        public CommandMonitor(ICommandLogger commandLogger)
        {
            _channelOutput = new ChannelOutput();
            commandLogger.Add(_channelOutput);
        }

        /// <summary>Waits for exit result.</summary>
        public string WaitForResult(string endMonitorWildcard, string[] skipLinesWildcards, Action<OutputMessage> onRead)
        {
            OutputMessage output;
            string message;
            do
            {
                output = _channelOutput.WaitAndRead();
                message = TrimEnd(output.Message, Environment.NewLine);
                if (string.IsNullOrEmpty(message))
                {
                    continue;
                }

                if (MatchWildCard(message, endMonitorWildcard))
                {
                    break;
                }

                if (skipLinesWildcards != null &&
                    skipLinesWildcards.Any(card => MatchWildCard(message, card)))
                {
                    continue;
                }

                onRead?.Invoke(output);
            }
            while (true);

            return message;
        }

        private static string TrimEnd(string input, string value) =>
                input.EndsWith(value, StringComparison.InvariantCulture) ?
                input.Substring(0, input.Length - value.Length) :
                input;

        private static bool MatchWildCard(string input, string pattern)
        {
            if (pattern[0] == '*')
            {
                return input.EndsWith(pattern.Substring(1), StringComparison.InvariantCultureIgnoreCase);
            }

            var lastIndex = pattern.Length - 1;
            if (pattern[lastIndex] == '*')
            {
                return input.StartsWith(pattern.Substring(0, lastIndex), StringComparison.InvariantCultureIgnoreCase);
            }

            return input.Equals(pattern, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
