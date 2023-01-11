using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory() { HostName = "127.0.0.1" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
{
    channel.QueueDeclare(queue: "Fila1", durable: false, exclusive: false, autoDelete: false, arguments: null);
    channel.QueueDeclare(queue: "Fila2", durable: false, exclusive: false, autoDelete: false, arguments: null);

    var consumer1 = new EventingBasicConsumer(channel);
    var consumer2 = new EventingBasicConsumer(channel);

    consumer1.Received += (model, ea) =>
    {
        try
        {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body.ToArray());
            Console.WriteLine($"[x] Consumer1 - Received: {message}");

            channel.BasicAck(ea.DeliveryTag, false);
        }
        catch
        {
            channel.BasicNack(ea.DeliveryTag, false, true);
        }
        Thread.Sleep(200);
    };

    consumer2.Received += (model, ea) =>
    {
        try
        {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body.ToArray());
            Console.WriteLine($"[x] Consumer2 -  Receibed: {message}");

            channel.BasicAck(ea.DeliveryTag, false);
        }
        catch
        {
            channel.BasicNack(ea.DeliveryTag, false, true);
        }
        Thread.Sleep(200);
    };

    channel.BasicConsume(queue: "Fila1", autoAck: false, consumer: consumer1);
    channel.BasicConsume(queue: "Fila2", autoAck: false, consumer: consumer2);

    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
}
