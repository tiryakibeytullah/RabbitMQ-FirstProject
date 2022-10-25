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
        //İlk olarak subscriber çalıştırılması durumunda, header-exchange olmayacağından sebep burada hata alınır. Bunun önüne geçilmesi için eğer yok ise bu alanda tekrardan oluşturma işlemi gerçekleştirilmiştir.
        channel.ExchangeDeclare("header-exchange", type: ExchangeType.Headers, durable: true);
        channel.BasicQos(0, 1, false);

        var consumer = new EventingBasicConsumer(channel);
        var queueName = channel.QueueDeclare().QueueName;
        Dictionary<string, object> headers = new();
        headers.Add("format", "pdf");
        headers.Add("shape", "A4");
        headers.Add("x-match", "all"); //all değeri alındığında mutlaka tüm key value değerleri eşlemesi gerekir.
        //headers.Add("x-match", "any"); ////any değeri alındığında bir adet key value değeri eşlemesi yeterli.

        channel.QueueBind(queueName, "header-exchange", String.Empty, headers);
        channel.BasicConsume(queueName, false, consumer);

        Console.WriteLine("Loglar dinlenmeye başlanıldı..");

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
