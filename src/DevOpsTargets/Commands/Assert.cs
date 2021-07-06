using System;

using static DevOpsTargets.Targets;

namespace DevOpsTargets
{
    /// <summary>General assert class.</summary>
    public static class Guard
    {
        /// <summary>Gets or sets a value indicating whether [exit when assert fail].</summary>
        public static bool ExitWhenAssertFail { get; set; } = true;

        /// <summary>Gets or sets the on fail action.</summary>
        public static Action OnFailAction { get; set; }

        /// <summary>Pass assert when the specified condition is met.</summary>
        public static void Truthy(bool condition, string error, string description = "The provided value must be true.")
        {
            WriteLine(description, LogLevel.Debug);
            if (!condition)
            {
                Fail(error);
            }
        }

        /// <summary>Fails with the specified error.</summary>
        public static void Fail(string error)
        {
            WriteLine(string.Empty);
            WriteLine(error, LogLevel.Error);
            if (ExitWhenAssertFail)
            {
                WriteLine("FAIL");
                try
                {
                    OnFailAction?.Invoke();
                }
                finally
                {
                    Environment.Exit(1);
                }
            }
        }
    }
}
