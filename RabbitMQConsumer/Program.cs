using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace RabbitMQConsumer
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

            channel.QueueDeclare("MyQueue", durable: true, autoDelete: false, exclusive: false, arguments: null);
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, basicDeliverEventArgs) =>
        {
            var body = basicDeliverEventArgs.Body;

            Console.WriteLine($"Received Message: {Encoding.UTF8.GetString(body)}");
            Thread.Sleep(2000);

            channel.BasicAck(basicDeliverEventArgs.DeliveryTag, false);
        };

            channel.BasicConsume(queue: "MyQueue", autoAck: false, consumer: consumer);
            Console.WriteLine("Waiting for message ....");
            Console.ReadLine();
        }

        //private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        //{
        //    var body = e.Body;

        //    Console.WriteLine($"Received Message: {Encoding.UTF8.GetString(body)}");

        //    Thread.Sleep(2000);

        //}
    }
}
