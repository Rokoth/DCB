using Microsoft.Extensions.Logging;

namespace DCB.Client
{
    public class ClientErrorNotifyLogger : Microsoft.Extensions.Logging.ILogger
    {
        private readonly string _name;
        private IClientErrorNotifyService _errorNotifyService;
        private readonly Func<ClientErrorNotifyLoggerConfiguration> _getCurrentConfig;

        public ClientErrorNotifyLogger(string name, IClientErrorNotifyService errorNotifyService,
            Func<ClientErrorNotifyLoggerConfiguration> getCurrentConfig)
        {
            _errorNotifyService = errorNotifyService;
            _getCurrentConfig = getCurrentConfig;
            _name = name;
        }

        public IDisposable BeginScope<TState>(TState state) => default;

        public bool IsEnabled(LogLevel logLevel)
        {
            return _getCurrentConfig().LogLevels.Contains(logLevel);
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            ClientErrorNotifyLoggerConfiguration config = _getCurrentConfig();
            if (config.EventId == 0 || config.EventId == eventId.Id)
            {
                try
                {
                    _errorNotifyService
                        .Send($"Message: {exception.Message} StackTrace: {exception.StackTrace}")
                        .ContinueWith(s => {
                            if (s.Exception != null)
                            {
                                Console.WriteLine($"ErrorNotify exception: {s.Exception.Message} {s.Exception.StackTrace}");
                            }
                        })
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ErrorNotify exception: {ex.Message} {ex.StackTrace}");
                }
            }
        }
    }
}