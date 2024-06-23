using System;
using System.IO;
using DCB.Common;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace DCB
{
    public class Program
    {
        private const string _logDirectory = "Logs";
        private const string _logFileName = "log-startup.txt";
        private const string _appSettingsFileName = "appsettings.json";
        private const string _startUpInfoMessage = "App starts with arguments: {0}";
        private const string _errorNotifyOptionsSection = "ErrorNotifyOptions";

        public static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            string _startUpLogPath = Path.Combine(_logDirectory, _logFileName);
            var loggerConfig = new LoggerConfiguration()
               .WriteTo.Console()
               .WriteTo.File(_startUpLogPath)
               .MinimumLevel.Verbose();

            using var logger = loggerConfig.CreateLogger();
            logger.Information(string.Format(_startUpInfoMessage, string.Join(", ", args)));

            GetWebHostBuilder(args).Build().Run();
        }

        protected static IWebHostBuilder GetWebHostBuilder(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(GetConfiguration())
                .ConfigureAppConfiguration((hostingContext, config) => ConfigureApp(args, config))
                .ConfigureLogging(CreateLogger)
                .UseKestrel()
                .UseStartup<Startup>();

            return builder;
        }

        private static void CreateLogger(WebHostBuilderContext hostingContext, ILoggingBuilder logging)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(hostingContext.Configuration)
                .CreateLogger();
            logging.AddSerilog(Log.Logger);
            logging.AddErrorNotifyLogger(config =>
            {
                config.Options = hostingContext.Configuration
                    .GetSection(_errorNotifyOptionsSection)
                    .Get<ErrorNotifyOptions>();
            });
        }

        /// <summary>
        /// Configure App method
        /// </summary>
        /// <param name="args"></param>
        /// <param name="config"></param>
        private static void ConfigureApp(string[] args, IConfigurationBuilder config)
        {
            if (args != null) config.AddCommandLine(args);

        }

        /// <summary>
        /// Build app Configuration
        /// </summary>
        /// <returns></returns>
        private static IConfigurationRoot GetConfiguration()
        {
            return new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile(_appSettingsFileName, optional: false, reloadOnChange: true)
                                .AddEnvironmentVariables()
                                .AddDbConfiguration()
                                .Build();
        }
    }
}
