using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;

using Options = Bullseye.Options;

namespace DevOpsTargets
{
    /// <summary>Targets main class.</summary>
    public static partial class Targets
    {
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

            cmd.Handler = CommandHandler.Create(() =>
            {
                // Translate from System.CommandLine to Bullseye.
                var cmdLine = cmd.Parse(args);
                var targets = cmdLine.CommandResult.Tokens.Select(token => token.Value);
                var options = new Options(Options.Definitions.Select(o => (o.LongName, cmdLine.ValueForOption<bool>(o.LongName))));

                InitHostOutput(options, prefix, Console.Out);
                Bullseye.Targets.RunTargetsAndExit(targets, options, null, prefix);
            });

            cmd.Invoke(args);
        }
    }
}
