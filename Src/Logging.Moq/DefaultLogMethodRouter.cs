using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace su3dev.Logging.Moq
{
    public class DefaultLogMethodRouter : ILogMethodRouter
    {
        public object Target { get; }

        public DefaultLogMethodRouter(object target)
        {
            Target = target;
        }
        
        public virtual void RouteCall<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception)
        {
            var methodName = GetLogMethodName(logLevel);
            if (string.IsNullOrWhiteSpace(methodName))
            {
                return;
            }

            var parameters = GetLogMethodParameters(eventId, state, exception);
            InvokeLogMethod(methodName, parameters);
        }

        protected virtual string GetLogMethodName(LogLevel logLevel)
        {
            var logMethodName = logLevel switch
            {
                LogLevel.Information => "LogInformation",
                LogLevel.Trace => "LogTrace",
                LogLevel.Debug => "LogDebug",
                LogLevel.Warning => "LogWarning",
                LogLevel.Error => "LogError",
                LogLevel.Critical => "LogCritical",
                _ => string.Empty
            };

            return logMethodName;
        }

        protected virtual object[] GetLogMethodParameters<TState>(EventId eventId, TState state, Exception? exception)
        {
            var statePairs = ExtractStatePairs(state);
            var originalFormat = GetOriginalFormat(statePairs);
            var args = AssembleArgs(statePairs);

            // NOTE: The element order in the parameter array is significant and must be as follows:
            // { eventId, exception, originalFormat, args }
            // In order to correctly select the simplest log method overload, certain elements are
            // not added when they have the following special values:
            // - event id, if the id is 0
            // - exception, if it is null
            // - args, if it is empty

            var parameterList = new List<object> { originalFormat };
            if (args.Any())
            {
                parameterList.Add(args);
            }
            
            if (exception is not null)
            {
                parameterList.Insert(0, exception);
            }

            if (eventId.Id != 0)
            {
                parameterList.Insert(0, eventId);
            }

            var parameters = parameterList.ToArray();
           
            return parameters;
        }

        protected virtual object? InvokeLogMethod(string methodName, object[] parameters)
        {
            var parameterTypes = parameters.Select(p => p.GetType()).ToArray();
            var methodInfo = Target.GetType().GetMethod(methodName, parameterTypes);
            return methodInfo?.Invoke(Target, parameters);
        }
        
        private static KeyValuePair<string, object>[] ExtractStatePairs<TState>(TState state)
        {
            var pairs = new List<KeyValuePair<string, object>>();
            if (state is IEnumerable stateAsEnumerable)
            {
                foreach (var item in stateAsEnumerable)
                {
                    if (item is KeyValuePair<string, object> pair)
                    {
                        pairs.Add(pair);
                    }
                }
            }

            return pairs.ToArray();
        }

        private static string GetOriginalFormat(IEnumerable<KeyValuePair<string, object>> statePairs)
        {
            var originalFormatPair = statePairs
                .FirstOrDefault(kvp => kvp.Key == Logger.OriginalFormatKey); 
            var originalFormat = originalFormatPair
                .Value?
                .ToString() ?? Logger.NullOriginalFormatValue;
            return originalFormat;
        }

        private static object[] AssembleArgs(IEnumerable<KeyValuePair<string, object>> statePairs)
        {
            var args = statePairs
                .Where(kvp => kvp.Key != Logger.OriginalFormatKey)
                .Select(kvp => kvp.Value)
                .ToArray();
            return args;
        }
    }
}
