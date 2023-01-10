using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace EstudoRabbitMQConsumer
{
    internal class Program
    {
        static void Main()
        {
            var factory = new ConnectionFactory() { HostName = "127.0.0.1" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "Hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());
                    Console.WriteLine($"[X] Received: {message}");

                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch
                {
                    var retryQueue = true;
                    channel.BasicNack(ea.DeliveryTag, false, retryQueue);
                }

                Thread.Sleep(200);
            };
            channel.BasicConsume(queue: "Hello", autoAck: false, consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}