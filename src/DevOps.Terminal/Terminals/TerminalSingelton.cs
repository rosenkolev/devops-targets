﻿using DevOps.Terminal.Loggers.Abstraction;

namespace DevOps.Terminal.Terminals
{
    /// <summary>The terminal singelton.</summary>
    public static class TerminalSingelton
    {
        /// <summary>The default instance.</summary>
        private static Terminal defaultInstance;

        /// <summary>Gets the default terminal.</summary>
        public static Terminal DefaultTerminal =>
            defaultInstance ??
            ResetDefaultTerminal();

        /// <summary>Resets the default.</summary>
        public static Terminal ResetDefaultTerminal()
        {
            defaultInstance = new Terminal(LogLevel.Verbose, null);
            return defaultInstance;
        }
    }
}
