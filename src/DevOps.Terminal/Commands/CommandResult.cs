using System.Diagnostics.CodeAnalysis;

namespace DevOps.Terminal.Commands
{
    /// <summary>The result of command or terminal execute.</summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "record")]
    public record CommandResult(string Output, int ExitCode);
}
