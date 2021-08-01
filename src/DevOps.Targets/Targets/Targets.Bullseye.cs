using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;

using DevOps.Terminal;

using HostLogLevel = DevOps.Terminal.Loggers.Abstraction.LogLevel;

using Options = Bullseye.Options;
using Result = System.CommandLine.Parsing.ParseResult;

namespace DevOps
{
    /// <summary>Targets main class.</summary>
    public static partial class Targets
    {
        /// <summary>The extra command line options.</summary>
        internal static readonly List<Option> ExtraOptions = new List<Option>();

        /// <summary>Gets the command line options result.</summary>
        public static Result OptionsResults { get; private set; }

        /// <summary>Extra command line option.</summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        public static Option Option<TValue>(string name, string description = null, TValue defaultValue = default, string shortName = null)
        {
            var typeOfValue = typeof(TValue);
            var alias = shortName != null ? new[] { name, shortName } : new[] { name };
            var option = new Option(alias, description, typeOfValue, () => defaultValue);
            ExtraOptions.Add(option);
            return option;
        }

        /// <summary>Value for option.</summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        public static TValue ValueForOption<TValue>(string name) =>
            OptionsResults.ValueForOption<TValue>(name);

        /// <summary>Value for option.</summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        public static TValue ValueForOption<TValue>(Option option) =>
            OptionsResults.ValueForOption<TValue>(option);

        /// <summary>Runs the specified targets arguments.</summary>
        public static void RunAndExit(IEnumerable<string> cmdArgs, string prefix = nameof(Targets))
        {
            var args = cmdArgs.ToArray();
            var cmd = new RootCommand
            {
                new Argument("targets")
                {
                    Arity = ArgumentArity.ZeroOrMore,
                    Description = "A list of targets to run or list. If not specified, the \"default\" target will be run, or all targets will be listed.",
                },
            };

            // translate from Bullseye to System.CommandLine
            foreach (var option in Options.Definitions)
            {
                cmd.Add(new Option(new[] { option.ShortName, option.LongName }.Where(n => !string.IsNullOrWhiteSpace(n)).ToArray(), option.Description));
            }

            ExtraOptions.ForEach(o => cmd.Add(o));

            cmd.Handler = CommandHandler.Create(() =>
            {
                // Translate from System.CommandLine to Bullseye.
                OptionsResults = cmd.Parse(args);
                var targets = OptionsResults.CommandResult.Tokens.Select(token => token.Value);
                var options = new Options(Options.Definitions.Select(o => (o.LongName, OptionsResults.ValueForOption<bool>(o.LongName))));
                var logOutput = options.Verbose ? HostLogLevel.Debug : HostLogLevel.Verbose;

                Out.ReInitConsoleOutput(logOutput, prefix);
                Bullseye.Targets.RunTargetsAndExit(targets, options, null, prefix);
            });

            cmd.Invoke(args);
        }
    }
}
