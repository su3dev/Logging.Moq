using Microsoft.Extensions.Logging;
// ReSharper disable CheckNamespace

namespace Moq
{
    public record LoggedMessage
    {
        public LoggedMessage(LogLevel level, EventId eventId, string? text, object? exception = null)
        {
            Level = level;
            Text = text;
            EventId = eventId;
            Exception = exception;
        }
        
        public LogLevel Level { get; }
        public EventId EventId { get; }
        public string? Text { get; }
        public object? Exception { get; }
    }
}
