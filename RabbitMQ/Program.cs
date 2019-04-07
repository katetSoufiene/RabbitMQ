using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;

namespace RabbitMQ
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.UserName = "guest";
            factory.Password = "guest";
            factory.HostName = "localhost";

            var conn = factory.CreateConnection();
            var channel = conn.CreateModel();
            var properties = channel.CreateBasicProperties();
            properties.DeliveryMode = 2;

            channel.ExchangeDeclare("MyExchange", ExchangeType.Direct);
            channel.QueueDeclare("MyQueue", durable: true, autoDelete: false, exclusive: false, arguments: null);

            channel.QueueBind("MyQueue", "MyExchange", "routing key");

            var randon = new Random();

            while (true)
            {
                //var message = $"Message #:{i}:{Guid.NewGuid()} ";
                //var messageBodyBytes = Encoding.UTF8.GetBytes(message);

                var randomId = randon.Next(1, 1000);

                var message = new Message { Object = $"Message object {randomId}", Body = $"Message body {randomId}" };
                var stringMessage = JsonConvert.SerializeObject(message);
                var messageBodyBytes = Encoding.UTF8.GetBytes(stringMessage);

                channel.BasicPublish("MyExchange", "routing key", properties, messageBodyBytes);

                Console.WriteLine($"message: {message.Object}");

                Thread.Sleep(1000);
            }
            //channel.Dispose();
            //conn.Dispose();

        }
    }
}
