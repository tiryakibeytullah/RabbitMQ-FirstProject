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

        //Kuyruğa göndericeğimiz için aynı isimlendirmeleri kullanalım. Random olmasın, kuyruk kalıcı olacak.
        //var randomQueueName = channel.QueueDeclare().QueueName;
        var randomQueueName = "log-database-save";

        //Eğer gönderilen mesajları (logları) kalıcı hale getirmek istiyorsak, bir kuyruk oluşturmamız gerek;
        channel.QueueDeclare(randomQueueName, true, false, false);

        //İlgili subscriber kapandığı taktirde kuyruğun sonlanması (silinmesi gibi düşünülebilir) için kuyruğu buradan kaldırıyoruz.
        channel.QueueBind(randomQueueName, "logs-fanout", "", null);

        var consumer = new EventingBasicConsumer(channel);
        channel.BasicConsume(randomQueueName, false, consumer);

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
