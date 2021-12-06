using System;
using Microsoft.Extensions.Logging;
using Moq;

namespace su3dev.Logging.Moq
{
    public class LoggerMock
    {
        public static Mock<Logger> Create()
        {
            var mock = new Mock<Logger> { CallBase = true };
            return mock;
        }

        public static Mock<Logger<T>> Create<T>()
            where T : class
        {
            var extendedMock = new Mock<Logger<T>> { CallBase = true };
            return extendedMock;
        }

        public static Mock<Logger<T>> Get<T>(ILogger<T> logger)
            where T : class
        {
            if (logger is not Logger<T> concreteLogger)
            {
                var message = GetExceptionMessage(logger.GetType(), typeof(Logger<T>));
                throw new ArgumentException(message);
            }
            
            var mock = Mock.Get(concreteLogger);
            return mock;
        }

        public static Mock<Logger> Get(ILogger logger)
        {
            if (logger is not Logger concreteLogger)
            {
                var message = GetExceptionMessage(logger.GetType(), typeof(Logger));
                throw new ArgumentException(message);
            }
            
            var mock = Mock.Get(concreteLogger);
            return mock; 
        }

        private static string GetExceptionMessage(Type passedType, Type expectedType)
        {
            var message = "Incompatible logger type. " +
                          $"The logger must be compatible with type {expectedType.FullName} in order for it to be properly mocked. " +
                          $"The logger that was passed in was of type {passedType.FullName}.";
            return message;
        }
    }
}
