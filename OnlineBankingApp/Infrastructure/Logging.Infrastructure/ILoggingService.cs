namespace Logging.Infrastructure
{
    public interface ILoggingService
    {
        void Information(string message, object model);
        void Warning(string message, object model);
        void Error(Exception ex, object model);
    }
}