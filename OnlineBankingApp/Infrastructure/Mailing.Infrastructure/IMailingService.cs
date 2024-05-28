namespace Mailing.Infrastructure
{
    public interface IMailingService
    {
        void Send(string message, string attachment = "");
    }
}