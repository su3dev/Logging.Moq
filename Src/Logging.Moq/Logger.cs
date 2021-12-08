using System;
using Microsoft.Extensions.Logging;

namespace su3dev.Logging.Moq
{
    public abstract partial class Logger : ILogger
    {
        public const string OriginalFormatKey = "{OriginalFormat}";
        public const string NullOriginalFormatValue = "[null]";
        
        public abstract IDisposable BeginScope<TState>(TState state);
        public abstract bool IsEnabled(LogLevel logLevel);
        
        public ILogMethodRouter LogMethodRouter { get; set; }

        protected Logger()
        {
            LogMethodRouter = new DefaultLogMethodRouter(this);
        }
        
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
        {
            LogMethodRouter.RouteCall(logLevel, eventId, state, exception);
        }
    }
}
