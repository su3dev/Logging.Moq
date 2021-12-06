using System;
using Microsoft.Extensions.Logging;

namespace su3dev.Logging.Moq
{
    public abstract partial class Logger
    {
        public abstract void LogInformation(string message);
        public abstract void LogInformation(EventId eventId, string message);
        public abstract void LogInformation(Exception exception, string message);
        public abstract void LogInformation(EventId eventId, Exception exception, string message);
        public abstract void LogInformation(string message, params object[] args);
        public abstract void LogInformation(EventId eventId, string message, params object[] args);
        public abstract void LogInformation(Exception exception, string message, params object[] args);
        public abstract void LogInformation(EventId eventId, Exception exception, string message, params object[] args);
        public abstract void LogTrace(string message);
        public abstract void LogTrace(EventId eventId, string message);
        public abstract void LogTrace(Exception exception, string message);
        public abstract void LogTrace(EventId eventId, Exception exception, string message);
        public abstract void LogTrace(string message, params object[] args);
        public abstract void LogTrace(EventId eventId, string message, params object[] args);
        public abstract void LogTrace(Exception exception, string message, params object[] args);
        public abstract void LogTrace(EventId eventId, Exception exception, string message, params object[] args);
        public abstract void LogDebug(string message);
        public abstract void LogDebug(EventId eventId, string message);
        public abstract void LogDebug(Exception exception, string message);
        public abstract void LogDebug(EventId eventId, Exception exception, string message);
        public abstract void LogDebug(string message, params object[] args);
        public abstract void LogDebug(EventId eventId, string message, params object[] args);
        public abstract void LogDebug(Exception exception, string message, params object[] args);
        public abstract void LogDebug(EventId eventId, Exception exception, string message, params object[] args);
        public abstract void LogWarning(string message);
        public abstract void LogWarning(EventId eventId, string message);
        public abstract void LogWarning(Exception exception, string message);
        public abstract void LogWarning(EventId eventId, Exception exception, string message);
        public abstract void LogWarning(string message, params object[] args);
        public abstract void LogWarning(EventId eventId, string message, params object[] args);
        public abstract void LogWarning(Exception exception, string message, params object[] args);
        public abstract void LogWarning(EventId eventId, Exception exception, string message, params object[] args);
        public abstract void LogError(string message);
        public abstract void LogError(EventId eventId, string message);
        public abstract void LogError(Exception exception, string message);
        public abstract void LogError(EventId eventId, Exception exception, string message);
        public abstract void LogError(string message, params object[] args);
        public abstract void LogError(EventId eventId, string message, params object[] args);
        public abstract void LogError(Exception exception, string message, params object[] args);
        public abstract void LogError(EventId eventId, Exception exception, string message, params object[] args);
        public abstract void LogCritical(string message);
        public abstract void LogCritical(EventId eventId, string message);
        public abstract void LogCritical(Exception exception, string message);
        public abstract void LogCritical(EventId eventId, Exception exception, string message);
        public abstract void LogCritical(string message, params object[] args);
        public abstract void LogCritical(EventId eventId, string message, params object[] args);
        public abstract void LogCritical(Exception exception, string message, params object[] args);
        public abstract void LogCritical(EventId eventId, Exception exception, string message, params object[] args);
    }
}

