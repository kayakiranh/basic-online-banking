using Auth.Application.DataTransferObjects;
using Auth.Application.Interfaces;
using Auth.Messaging.Send.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Auth.Messaging.Send.Sender
{
    public class AccountOpenSender : IAccountOpenSender
    {
        private readonly string _hostname;
        private readonly string _password;
        private readonly string _queueName;
        private readonly string _username;
        private IConnection _connection;

        public AccountOpenSender(IOptions<RabbitMqConfiguration> rabbitMqOptions)
        {
            _queueName = rabbitMqOptions.Value.QueueName;
            _hostname = rabbitMqOptions.Value.Hostname;
            _username = rabbitMqOptions.Value.UserName;
            _password = rabbitMqOptions.Value.Password;

            CreateConnection();
        }

        public void OpenAccount(RegisterRequestDto model)
        {
            if (ConnectionExists())
            {
                using (IModel channel = _connection.CreateModel())
                {
                    channel.QueueDeclare(queue: _queueName, true, false, false, null);
                    string json = JsonConvert.SerializeObject(model);
                    byte[] body = Encoding.UTF8.GetBytes(json);

                    channel.BasicPublish(exchange: string.Empty, routingKey: _queueName, basicProperties: null, body: body);
                }
            }
        }

        private void CreateConnection()
        {
            try
            {
                ConnectionFactory factory = new ConnectionFactory
                {
                    HostName = _hostname,
                    UserName = _username,
                    Password = _password
                };
                _connection = factory.CreateConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not create connection: {ex.Message}");
            }
        }

        private bool ConnectionExists()
        {
            if (_connection != null)
            {
                return true;
            }

            CreateConnection();

            return _connection != null;
        }
    }
}