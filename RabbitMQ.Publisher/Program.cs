using RabbitMQ.Client;
using Shared;
using System.Text;
using System.Text.Json;

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
        properties.Persistent = true; //True: Mesajlarımız kalıcı hale gelir. 

        var product = new Product() { Id = 1, Name = "Defter", Price = 31, Stock = 100 };
        var productJsonString = JsonSerializer.Serialize(product);

        channel.BasicPublish("header-exchange", String.Empty, properties, Encoding.UTF8.GetBytes(productJsonString));

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