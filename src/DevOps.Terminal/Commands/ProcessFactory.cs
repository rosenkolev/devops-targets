using System.Diagnostics;

namespace DevOps.Terminal.Commands
{
    /// <summary>The process factory.</summary>
    public static class ProcessFactory
    {
        /// <summary>Creates the specified process.</summary>
        public static Process Create(ProcessStartInfo info, ICommandLogger logger)
        {
            var process = new Process
            {
                StartInfo = info,
            };

            if (logger != null)
            {
                if (info.RedirectStandardOutput)
                {
                    process.OutputDataReceived += logger.LogOutput;
                }

                if (info.RedirectStandardError)
                {
                    process.ErrorDataReceived += logger.LogOutput;
                }
            }

            return process;
        }
    }
}
