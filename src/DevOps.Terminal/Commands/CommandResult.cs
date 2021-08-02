﻿namespace DevOps.Terminal.Commands
{
    /// <summary>The result of command or terminal execute.</summary>
    public record CommandResult(string Output, int ExitCode);
}
