using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Moq;

namespace su3dev.Logging.Moq
{
    public class LoggerMock
    {
        private static readonly MethodInfo CreateOfTOpen = typeof(LoggerMock).GetMethod(nameof(Create), 1, Type.EmptyTypes);
        
        public static Mock<Logger> Create()
        {
            var mock = new Mock<Logger> { CallBase = true };
            return mock;
        }

        public static Mock<Logger<T>> Create<T>()
            where T : class
        {
            var mock = new Mock<Logger<T>> { CallBase = true };
            return mock;
        }
        
        public static T Of<T>()
            where T : class, ILogger
        {
            Mock? mock = null;
            
            var specifiedType = typeof(T);

            var isILogger = specifiedType == typeof(ILogger);
            if (isILogger)
            {
                mock = Create();
            }
            else
            {
                var loggerOfTType = typeof(ILogger<>);
                var underlyingT = specifiedType.GenericTypeArguments.FirstOrDefault();
                if (underlyingT is not null)
                {
                    var constructedType = loggerOfTType.MakeGenericType(underlyingT);
                    var isILoggerOfT = specifiedType == constructedType;
                    if (isILoggerOfT)
                    {
                        mock = CreateFromCategory(underlyingT);
                    }
                }
            }
            
            if (mock is null)
            {
                var message =
                    $"Cannot create mock of {typeof(T).FullName}. The specified type needs to be {typeof(ILogger).FullName} or {typeof(ILogger<>).FullName}.";
                throw new ArgumentException(message);
            }
            
            var typed = mock.Object as T;
            return typed!;
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
        
        private static Mock CreateFromCategory(Type category)
        {
            var createOfT = CreateOfTOpen.MakeGenericMethod(category);
            var created = createOfT.Invoke(null, null);
            var logger = created as Mock;
            return logger!;
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
