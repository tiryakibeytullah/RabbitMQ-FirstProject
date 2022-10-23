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

        channel.ExchangeDeclare("logs-fanout", type: ExchangeType.Fanout, durable: true);

        Enumerable.Range(1, 50).ToList().ForEach(x =>
        {
            string message = $"Logs: {x}";
            var messageBody = Encoding.UTF8.GetBytes(message);

            //Artık elimizde bir exchange ismi olduğundan, Publish işleminde bu exchange ismi verilmeli.
            channel.BasicPublish("logs-fanout", "", null, messageBody);

            Console.WriteLine($"{x}.mesajınız gönderilmiştir.");
        });

        Console.ReadLine();
    }
}