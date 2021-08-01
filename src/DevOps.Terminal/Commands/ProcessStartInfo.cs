using System;

using StartInfo = System.Diagnostics.ProcessStartInfo;

namespace DevOps.Terminal.Commands
{
    public static class ProcessStartInfoFactory
    {
        public static StartInfo Create(string command, string arguments, string workingDirectory)
        {
            Validate(command);
            var startInfo = new StartInfo(command)
            {
                CreateNoWindow = true,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
            };

            if (!string.IsNullOrEmpty(workingDirectory))
            {
                startInfo.WorkingDirectory = workingDirectory;
            }

            return startInfo;
        }

        private static void Validate(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The command name is missing.", nameof(name));
            }
        }
    }
}
