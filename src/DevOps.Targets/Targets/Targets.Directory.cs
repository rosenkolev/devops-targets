using System.IO;
using System.Runtime.CompilerServices;

namespace DevOps
{
    /// <summary>Targets main class.</summary>
    public static partial class Targets
    {
        /// <summary>Gets the script folder.</summary>
        public static string GetScriptFolder([CallerFilePath] string path = null) => Path.GetDirectoryName(path);

        /// <summary>Ensures the directory exists.</summary>
        public static void EnsureDirectoryExists(string pathToDirectory)
        {
            if (!Directory.Exists(pathToDirectory))
            {
                Directory.CreateDirectory(pathToDirectory);
            }
        }

        /// <summary>Cleans the directory.</summary>
        public static void CleanDirectory(string pathToDirectory)
        {
            DeleteAllFilesAndFolders(pathToDirectory);
            Directory.CreateDirectory(pathToDirectory);
        }

        /// <summary>Deletes all files and folders.</summary>
        public static void DeleteAllFilesAndFolders(params string[] paths)
        {
            foreach (var path in paths)
            {
                if (File.Exists(path))
                {
                    WriteLine($"Delete file {path}.", LogLevel.Verbose);
                    File.Delete(path);
                }
                else if (Directory.Exists(path))
                {
                    WriteLine($"Delete directory {path}.", LogLevel.Verbose);
                    Directory.Delete(path, true);
                }
            }
        }
    }
}
