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

        channel.ExchangeDeclare("logs-topic", type: ExchangeType.Topic, durable: true);

        Enumerable.Range(1, 50).ToList().ForEach(x =>
        {
            Random rnd = new Random();
            LogNames firstLog = (LogNames)rnd.Next(1, 5);
            LogNames secondLog = (LogNames)rnd.Next(1, 5);
            LogNames thirdLog = (LogNames)rnd.Next(1, 5);

            var routeKey = $"{firstLog}.{secondLog}.{thirdLog}";
            string message = $"log-type: {firstLog}-{secondLog}-{thirdLog}";
            var messageBody = Encoding.UTF8.GetBytes(message);

            //Artık elimizde bir exchange ismi olduğundan, Publish işleminde bu exchange ismi verilmeli.
            channel.BasicPublish("logs-topic", routeKey, null, messageBody);

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