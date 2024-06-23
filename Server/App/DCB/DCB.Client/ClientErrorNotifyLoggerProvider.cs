using DCB.Client.Services.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace DCB.Client
{
    public sealed class ClientErrorNotifyLoggerProvider : ILoggerProvider
    {
        private readonly IDisposable _onChangeToken;
        private ClientErrorNotifyLoggerConfiguration _currentConfig;
        private readonly ConcurrentDictionary<string, ClientErrorNotifyLogger> _loggers = new ConcurrentDictionary<string, ClientErrorNotifyLogger>();


        public ClientErrorNotifyLoggerProvider(
            IOptionsMonitor<ClientErrorNotifyLoggerConfiguration> config)
        {
            _currentConfig = config.CurrentValue;
            _onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig);
        }

        public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
        {
            var errorNotifyService = new ClientErrorNotifyService(new HttpClientService());
            var logger = _loggers.GetOrAdd(categoryName, name => new ClientErrorNotifyLogger(name, errorNotifyService, GetCurrentConfig));
            return logger;
        }

        private ClientErrorNotifyLoggerConfiguration GetCurrentConfig() => _currentConfig;

        public void Dispose()
        {
            _loggers.Clear();
            _onChangeToken.Dispose();
        }
    }
}