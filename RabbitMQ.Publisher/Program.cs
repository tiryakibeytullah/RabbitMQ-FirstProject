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
        channel.QueueDeclare("hello-queue", true, false, false);

        Enumerable.Range(1, 50).ToList().ForEach(x =>
        {
            string message = $"Message: {x}";
            var messageBody = Encoding.UTF8.GetBytes(message);

            //Herhangi bir Exchange belirtilmez ise routKey olaraktan channel'da oluşturulmuş olan kanal verilmelidir.
            channel.BasicPublish(string.Empty, "hello-queue", null, messageBody);

            Console.WriteLine($"{x}.mesajınız gönderilmiştir.");
        });

        Console.ReadLine();
    }
}