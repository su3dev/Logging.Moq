using System;
using Microsoft.Extensions.Logging;

namespace su3dev.Logging.Moq
{
    public interface ILogMethodRouter
    {
        void RouteCall<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception);
    }
}
