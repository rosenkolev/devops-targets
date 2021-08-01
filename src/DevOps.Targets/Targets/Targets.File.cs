using System;
using System.Globalization;
using System.IO;

namespace DevOps
{
    /// <summary>Targets main class.</summary>
    public static partial class Targets
    {
        /// <summary>Creates the temporary file.</summary>
        public static string GetTempFileName(string ext)
        {
            var fileName = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
            var filePath = Path.Combine(Path.GetTempPath(), fileName + ext);
            return filePath;
        }

        /// <summary>Gets the content from lines.</summary>
        public static string GetContentFromLines(params string[] lines) =>
            string.Join(Environment.NewLine, lines);
    }
}
