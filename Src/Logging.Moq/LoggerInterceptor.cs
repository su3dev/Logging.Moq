using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;

namespace su3dev.Logging.Moq
{
    public class LoggerInterceptor
    {
        public IEnumerable<LoggedMessage> LoggedMessages { get { return LoggedMessagesInternal.AsReadOnly(); } }

        private List<LoggedMessage> LoggedMessagesInternal { get; }

        public static LoggerInterceptor Create<TLogger>(Mock<TLogger> mock)
            where TLogger : class, ILogger
        {
            var interceptor = new LoggerInterceptor();

            mock
                .Setup(
                    m => m.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(),
                        It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()))
                .Callback(
                    (IInvocation i) =>
                    {
                        var level = (LogLevel) i.Arguments[0];
                        var eventId = (EventId) i.Arguments[1];
                        var text = i.Arguments[2]!.ToString();
                        var exception = i.Arguments[3];
                        interceptor.AddLoggedMessage(level, eventId, text, exception);
                    });

            return interceptor;
        }

        private void AddLoggedMessage(LogLevel level, EventId eventId, string text, object? exception)
        {
            var message = new LoggedMessage(level, eventId, text, exception);
            LoggedMessagesInternal.Add(message);
        }

        private LoggerInterceptor()
        {
            LoggedMessagesInternal = new List<LoggedMessage>();
        }
    }
}
