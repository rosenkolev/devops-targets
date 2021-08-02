using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace DevOps.Terminal.Commands
{
    /// <summary>Exit code is not valid exception.</summary>
    [Serializable]
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public sealed class ExitCodeException : Exception, ISerializable
    {
        /// <summary>Initializes a new instance of the <see cref="ExitCodeException"/> class.</summary>
        public ExitCodeException()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ExitCodeException"/> class.</summary>
        public ExitCodeException(int exitCode)
            : base($"Process exit code '{exitCode}' is invalid")
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ExitCodeException"/> class.</summary>
        private ExitCodeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <inheritdoc/>
        public override void GetObjectData(SerializationInfo info, StreamingContext context) =>
            base.GetObjectData(info, context);

        private string GetDebuggerDisplay() => ToString();
    }
}
