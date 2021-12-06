using Microsoft.Extensions.Logging;

namespace su3dev.Logging.Moq
{
    public abstract class Logger<T> : Logger, ILogger<T>
        where T : class
    { }
}
