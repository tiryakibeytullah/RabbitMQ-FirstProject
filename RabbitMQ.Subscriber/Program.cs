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
        channel.BasicQos(0, 5, false);

        //Notes: Eğer okuma işleminde bu satır silinir ise, publisher'da böyle bir kuyruk yok ise hata alınır. Fakat silinmemesi durumunda böyle bir kuyruk yok ise en baştan oluşturulacağı için hata alınmaz.
        channel.QueueDeclare("hello-queue", true, false, false);

        var consumer = new EventingBasicConsumer(channel);
        channel.BasicConsume("hello-queue", false, consumer);
        consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
        {
            var message = Encoding.UTF8.GetString(e.Body.ToArray());

            Thread.Sleep(1000);
            Console.WriteLine($"Gelen mesaj: {message}");
            channel.BasicAck(e.DeliveryTag, false);
        };

        Console.ReadLine();
    }
}
