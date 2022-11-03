using Mango.MessageBus;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Mango.Services.OrderAPI.RabbitMQSender
{
    public class RabbitMQOrderMessageSender : IRabbitMQOrderMessageSender
    {
        private readonly string _hostname;
        private readonly string _username;
        private readonly string _password;

        private IConnection _connection;
        public RabbitMQOrderMessageSender()
        {
            _hostname = "localhost";
            _username = "guest";
            _password = "guest";
        }
        public void SendMessage(BaseMessage message, string queueName)
        {
            if (ConnectionExist())
            {
                // create channel where we send message
                using var channel = _connection.CreateModel();
                channel.QueueDeclare(queue: queueName, false, false, false, arguments: null);
                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);
                // publish message to a channel
                channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
            }
        }
        private void CreateConnection()
        {
            try
            {
                // Create connection
                var factory = new ConnectionFactory
                {
                    HostName = _hostname,
                    UserName = _username,
                    Password = _password
                };
                // establish connection
                _connection = factory.CreateConnection();
            }
            catch (Exception)
            {
                // Log exception
            }
        }
        private bool ConnectionExist()
        {
            if(_connection != null)
                return true;
            CreateConnection();
            return _connection != null;
        }
    }
}
