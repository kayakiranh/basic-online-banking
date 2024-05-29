using Serilog;
using System.Text.Json;

namespace Logging.Infrastructure
{
    [Serializable]
    public class LoggingService : ILoggingService
    {
        private readonly ILogger _logger;

        public LoggingService()
        {
            _logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
        }

        public void Error(Exception ex, object model)
        {
            _logger.Error(ex, JsonSerializer.Serialize(model));
        }

        public void Information(string message, object model)
        {
            _logger.Information($"Message : {message}, Model : {JsonSerializer.Serialize(model)}");
        }

        public void Warning(string message, object model)
        {
            _logger.Warning($"Message : {message}, Model : {JsonSerializer.Serialize(model)}");
        }
    }
}