using Account.Application.DataTransferObjects;
using Account.Application.Interfaces;
using Account.Messaging.Receive.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Account.Messaging.Receive.Receiver
{
    internal class AccountOpenReceiver : IAccountOpenReceiver
    {
        private readonly string _hostname;
        private readonly string _password;
        private readonly string _queueName;
        private readonly string _username;
        private IConnection _connection;

        public AccountOpenReceiver(IOptions<RabbitMqConfiguration> rabbitMqOptions)
        {
            _queueName = rabbitMqOptions.Value.QueueName;
            _hostname = rabbitMqOptions.Value.Hostname;
            _username = rabbitMqOptions.Value.UserName;
            _password = rabbitMqOptions.Value.Password;

            CreateConnection();
        }

        public void OpenAccount(AccountCreateResponseDto model)
        {
            if (ConnectionExists())
            {
                using (IModel channel = _connection.CreateModel())
                {
                    channel.QueueDeclare(_queueName, exclusive: false);
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, eventArgs) =>
                    {
                        var body = eventArgs.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine($"Product message received: {message}");
                    };
                    //read the message
                    channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
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