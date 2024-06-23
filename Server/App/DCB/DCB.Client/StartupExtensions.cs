using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace DCB.Client
{
    public static class StartupExtensions
    {
        public static ILoggingBuilder AddErrorNotifyLogger(
            this ILoggingBuilder builder,
            Action<ClientErrorNotifyLoggerConfiguration> configure)
        {
            builder.AddErrorNotifyLogger();
            builder.Services.Configure(configure);

            return builder;
        }

        public static ILoggingBuilder AddErrorNotifyLogger(
        this ILoggingBuilder builder)
        {
            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<ILoggerProvider, ClientErrorNotifyLoggerProvider>());

            LoggerProviderOptions.RegisterProviderOptions
                <ClientErrorNotifyLoggerConfiguration, ClientErrorNotifyLoggerProvider>(builder.Services);

            return builder;
        }
    }
}