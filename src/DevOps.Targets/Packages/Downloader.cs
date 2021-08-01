using System.IO;
using System.Net;

namespace DevOps.Packages
{
    /// <summary>Download files and tools from the web.</summary>
    public static class Downloader
    {
        /// <summary>Gets the tools directory.</summary>
        public static string ToolsDirectory => Path.Combine(Path.GetTempPath(), $"DevOpsTools");

        /// <summary>Gets the tool download path.</summary>
        public static string GetToolPath(string name, string version, string packageFilePath) =>
            Path.Combine(ToolsDirectory, $"{name}.{version}", packageFilePath);

        /// <summary>Downloads the file.</summary>
        public static string DownloadFile(string name, string version, string fileName, string url)
        {
            var pathToDirectory = GetToolPath(name, version, string.Empty);
            var pathToFile = Path.Combine(pathToDirectory, fileName);
            if (!File.Exists(pathToFile))
            {
                Targets.EnsureDirectoryExists(pathToDirectory);
                Targets.WriteLine($"Downloading {name} from {url} to {pathToDirectory}", Targets.LogLevel.Verbose);
                using var webClient = new WebClient();
                webClient.DownloadFile(url, pathToFile);
            }

            return pathToFile;
        }
    }
}
