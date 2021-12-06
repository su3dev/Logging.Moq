using System;
using System.Runtime.Serialization;

namespace su3dev.Logging.Moq.Tests
{
    [Serializable]
    public class UnitTestForcedException : Exception
    {
        public UnitTestForcedException()
        { }

        public UnitTestForcedException(string message)
            : base(message)
        { }

        public UnitTestForcedException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected UnitTestForcedException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        { }
    }
}
