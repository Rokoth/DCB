using Microsoft.Extensions.Logging;

namespace DCB.Client
{
    public class ClientErrorNotifyLoggerConfiguration
    {
        public int EventId { get; set; }       

        public List<LogLevel> LogLevels { get; set; } =
        [
            LogLevel.Error,
            LogLevel.Critical
        ];
    }
}