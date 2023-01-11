using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory() { HostName = "127.0.0.1" };
using var conecction = factory.CreateConnection();
using var channel = conecction.CreateModel();

channel.BasicQos(0, prefetchCount: 2, false);

var queueName1 = "ImageProcess";
var queueName2 = "ImageArchive";

channel.QueueDeclare(queue: queueName1, durable: false, exclusive: false, autoDelete: false, arguments: null);
channel.QueueDeclare(queue: queueName2, durable: false, exclusive: false, autoDelete: false, arguments: null);

var consumer1 = new EventingBasicConsumer(channel);
var consumer2 = new EventingBasicConsumer(channel);

consumer1.Received += (model, ea) =>
{
    try
    {
        var body = ea.Body;
        var message = Encoding.UTF8.GetString(body.ToArray());
        Console.WriteLine($"[x] Consumer1 - Process: {message}");

        channel.BasicAck(ea.DeliveryTag, false);
    }
    catch
    {
        channel.BasicNack(ea.DeliveryTag, false, true);
    }
    Thread.Sleep(50);
};

consumer2.Received += (model, ea) =>
{
    try
    {
        var body = ea.Body;
        var message = Encoding.UTF8.GetString(body.ToArray());
        Console.WriteLine($"[x] Consumer2 - Process: {message}");

        channel.BasicAck(ea.DeliveryTag, false);
    }
    catch
    {
        channel.BasicNack(ea.DeliveryTag, false, true);
    }
    Thread.Sleep(50);
};

channel.BasicConsume(queue: queueName1, autoAck: false, consumer: consumer1);
channel.BasicConsume(queue: queueName2, autoAck: false, consumer: consumer2);

Console.WriteLine(" Press [enter] to exit");
Console.ReadLine();
