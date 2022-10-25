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

        channel.ExchangeDeclare("header-exchange", type: ExchangeType.Headers, durable: true);

        Dictionary<string, object> headers = new();
        headers.Add("format", "pdf");
        headers.Add("shape", "A4");

        var properties = channel.CreateBasicProperties();
        properties.Headers = headers;

        channel.BasicPublish("header-exchange", String.Empty, properties, Encoding.UTF8.GetBytes("Header mesajı"));

        Console.WriteLine("Header mesajınız gönderilmiştir.");

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