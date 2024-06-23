using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace DCB.Client
{
    internal static class Program
    {
        private const string _logDirectory = "Logs";
        private const string _logFileName = "log-startup.txt";
        private const string _appSettingsFileName = "appsettings.json";
        private const string _startUpInfoMessage = "App starts with arguments: {0}";
               
        [STAThread]
        static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string _startUpLogPath = Path.Combine(_logDirectory, _logFileName);
            var loggerConfig = new LoggerConfiguration()
               .WriteTo.Console()
               .WriteTo.File(_startUpLogPath)
               .MinimumLevel.Verbose();

            using var logger = loggerConfig.CreateLogger();
            logger.Information(string.Format(_startUpInfoMessage, string.Join(", ", args)));

            var host = CreateHostBuilder(args).Build();
            ServiceProvider = host.Services;

            Application.Run(ServiceProvider.GetRequiredService<MainForm>());
        }
        public static IServiceProvider ServiceProvider { get; private set; }
       
        static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration(s => s.SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile(_appSettingsFileName, optional: false, reloadOnChange: true)
                                .AddEnvironmentVariables())              
                .ConfigureAppConfiguration((hostingContext, config) => ConfigureApp(args, config))
                .ConfigureLogging(CreateLogger)
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(services);
                });
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<MainForm>();
        }

        private static void CreateLogger(HostBuilderContext hostingContext, ILoggingBuilder logging)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(hostingContext.Configuration)
                .CreateLogger();
            logging.AddSerilog(Log.Logger);
            logging.AddErrorNotifyLogger();
        }

        private static void ConfigureApp(string[] args, IConfigurationBuilder config)
        {
            if (args != null) config.AddCommandLine(args);

        }
    }
}