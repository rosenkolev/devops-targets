﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DevOps.Terminal.Terminals
{
    public abstract class TerminalCommandSyntax
    {
        /// <summary>Gets the name of the command.</summary>
        public abstract string CommandName { get; }

        /// <summary>Gets the command arguments.</summary>
        public abstract string CommandArguments { get; }

        /// <summary>Gets the return code command.</summary>
        public abstract string ReturnCodeCommand { get; }

        /// <summary>Builds the command.</summary>
        public abstract string BuildCommand(string[] arguments);

        /// <summary>Clears the terminal inputs by building wildcards wildcards.</summary>
        public abstract IEnumerable<string> BuildInputClearWildCards(params string[] commands);

        /// <summary>Creates the argument string.</summary>
        protected string CreateArgumentString(string[] arguments, Action<string, StringBuilder> appendArgument)
        {
            var builder = new StringBuilder();
            var isFirstArgument = true;
            foreach (var argument in arguments)
            {
                if (isFirstArgument)
                {
                    isFirstArgument = false;
                }
                else
                {
                    builder.Append(' ');
                }

                appendArgument(argument, builder);
            }

            return builder.ToString();
        }
    }
}
