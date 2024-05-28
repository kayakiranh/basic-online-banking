using System.Net.Mail;
using System.Text;

namespace Mailing.Infrastructure
{
    [Serializable]
    public class MailingService : IMailingService
    {
        public void Send(string message, string attachment)
        {
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.Host = "mail.githubrepo.com";
            client.EnableSsl = false;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("noreply@githubrepo.com", "password");
            MailMessage mailMessage = new MailMessage("info@githubrepo.com", "info@githubrepo.com", "subject", message);
            mailMessage.IsBodyHtml = true;
            mailMessage.BodyEncoding = UTF8Encoding.UTF8;
            mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            if (!string.IsNullOrEmpty(attachment))
                mailMessage.Attachments.Add(new Attachment(attachment));

            client.Send(mailMessage);
        }
    }
}