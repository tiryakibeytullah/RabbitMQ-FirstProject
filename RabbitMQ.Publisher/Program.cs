using RabbitMQ.Client;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var conFactory = new ConnectionFactory();
        conFactory.Uri = new Uri("amqps://snahhpmk:oN7FJS_T4LkCBXcZu_pIj0_57DB4PNd5@hawk.rmq.cloudamqp.com/snahhpmk");

        using var connection = conFactory.CreateConnection();
        var channel = connection.CreateModel();

        channel.ExchangeDeclare("logs-direct", type: ExchangeType.Direct, durable: true);

        Enum.GetNames(typeof(LogNames)).ToList().ForEach(x =>
        {
            var routeKey = $"route-{x}";
            var queueName = $"direct-queue-{x}";
            channel.QueueDeclare(queueName, true, false, false);
            channel.QueueBind(queueName, "logs-direct", routeKey, null);
        });

        Enumerable.Range(1, 50).ToList().ForEach(x =>
        {
            //Rastgele log tipi seçilir.
            LogNames log = (LogNames)new Random().Next(1, 5);
            string message = $"log-type: {log}";
            var messageBody = Encoding.UTF8.GetBytes(message);
            var routeKey = $"route-{log}";

            //Artık elimizde bir exchange ismi olduğundan, Publish işleminde bu exchange ismi verilmeli.
            channel.BasicPublish("logs-direct", routeKey, null, messageBody);

            Console.WriteLine($"Log gönderilmiştir: {message}");
        });

        Console.ReadLine();
    }

    public enum LogNames
    {
        Critical = 1,
        Error = 2,
        Warning = 3,
        Info = 4
    }
}