using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var conFactory = new ConnectionFactory();
        conFactory.Uri = new Uri("amqps://snahhpmk:oN7FJS_T4LkCBXcZu_pIj0_57DB4PNd5@hawk.rmq.cloudamqp.com/snahhpmk");

        var connection = conFactory.CreateConnection();
        var channel = connection.CreateModel();
        channel.BasicQos(0, 1, false);

        var consumer = new EventingBasicConsumer(channel);
        var queueName = "direct-queue-Critical";
        channel.BasicConsume(queueName, false, consumer);

        Console.WriteLine("Loglar dinlenmeye başlanıldı..");

        consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
        {
            var message = Encoding.UTF8.GetString(e.Body.ToArray());

            Thread.Sleep(1000);
            Console.WriteLine($"Gelen mesaj: {message}");
            //File.AppendAllText("log-critical.txt", message + "\n");

            channel.BasicAck(e.DeliveryTag, false);
        };

        Console.ReadLine();
    }
}
