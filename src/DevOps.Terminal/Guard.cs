using System;

using DevOps.Terminal.Loggers.Abstraction;

namespace DevOps.Terminal
{
    /// <summary>General assert class.</summary>
    public static class Guard
    {
        /// <summary>Gets or sets a value indicating whether [exit when assert fail].</summary>
        public static bool ExitWhenAssertFail { get; set; } = true;

        /// <summary>Gets or sets the on fail action.</summary>
        public static Action OnFailAction { get; set; }

        /// <summary>Pass assert when the specified condition is met.</summary>
        public static void IsTrue(bool condition, Func<string> onError, string description = "The provided value must be true.")
        {
            Out.WriteLine(description, LogLevel.Debug);
            if (!condition)
            {
                Fail(onError?.Invoke());
            }
        }

        /// <summary>Pass assert when the specified condition is met.</summary>
        public static void IsTrue(bool condition, string error, string description = "The provided value must be true.") =>
            IsTrue(condition, () => error, description);

        /// <summary>Fails with the specified error.</summary>
        public static void Fail(string error)
        {
            Out.WriteLine(string.Empty);
            Out.WriteLine(error, LogLevel.Error);
            if (ExitWhenAssertFail)
            {
                Out.WriteLine("FAIL");
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
