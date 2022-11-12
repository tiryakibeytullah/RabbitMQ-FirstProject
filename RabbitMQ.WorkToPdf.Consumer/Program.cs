using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.WorkToPdf.Consumer;
using Spire.Doc;
using System.Net;
using System.Net.Mail;
using System.Text;

public class Program
{
    public static bool EmailSend(string email, MemoryStream memory, string fileName)
    {
        //Notes: GoogleHesap ayarları neticesinde mail gönderme işlemi sırasında Auth. takılmaktayız.
        try
        {
            memory.Position = 0;
            System.Net.Mime.ContentType contentType = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);

            Attachment attachment = new Attachment(memory, contentType);
            attachment.ContentDisposition.FileName = $"{fileName}.pdf";

            MailMessage mailMessage = new MailMessage();
            SmtpClient smtpClient = new SmtpClient();

            mailMessage.From = new MailAddress("tiryaki52341903@gmail.com");
            mailMessage.To.Add(email);
            mailMessage.Subject = "Pdf Dosyası Hk.";
            mailMessage.Body = $"{fileName} isimli Word dosyanız, Pdf dosyasına çevirilmiştir. İlgili dosyayı ekte bulabilirsiniz.";
            mailMessage.IsBodyHtml = true;

            mailMessage.Attachments.Add(attachment);

            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;

            NetworkCredential networkCredential = new NetworkCredential("denemeEmailAdresi", "denemeSifre");
            smtpClient.Credentials = networkCredential;
            smtpClient.Send(mailMessage);

            Console.WriteLine($"Sonuç: {email} adresine mesajınız gönderilmiştir.");

            memory.Close();
            memory.Dispose();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return false;
        }
    }
    private static void Main(string[] args)
    {
        var result = false;
        var factory = new ConnectionFactory();
        var queueName = "file-queue";
        var exchangeName = "convert-exchange";
        var routingKeyName = "word-to-pdf";
        var rabbitMQClientConnString = "amqps://snahhpmk:oN7FJS_T4LkCBXcZu_pIj0_57DB4PNd5@hawk.rmq.cloudamqp.com/snahhpmk";
        factory.Uri = new Uri(rabbitMQClientConnString);

        using (var connection = factory.CreateConnection())
        {
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false, null);
                channel.QueueBind(queueName, exchangeName, routingKeyName);
                channel.BasicQos(0, 1, false);

                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queueName, false, consumer);
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        Console.WriteLine("Kuyruktan 1 mesaj alındı. İşleniyor..");

                        var deserializeString = Encoding.UTF8.GetString(ea.Body.ToArray());
                        ResultMessageWordToPdf resultMessage = JsonConvert.DeserializeObject<ResultMessageWordToPdf>(deserializeString);

                        Document document = new Document();
                        document.LoadFromStream(new MemoryStream(resultMessage.WordByte), FileFormat.Docx2013);

                        using (var memoryStream = new MemoryStream())
                        {
                            document.SaveToStream(memoryStream, FileFormat.PDF);
                            result = EmailSend(resultMessage.Email, memoryStream, resultMessage.FileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception: {ex.Message}");
                        throw;
                    }

                    if (result)
                    {
                        //Dönüştürme ve mail gönderme işlemi başarılı ise, RabbitMQ'dan mesaj silinir.
                        Console.WriteLine("Kuyruktan okunan mesaj başarılı bir şekilde işlenmiştir.");
                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                    else
                    {
                        Console.WriteLine("Kuyruktan okunan mesaj sırasında bir hata meydana geldi.");
                    }
                };
                Console.WriteLine("Çıkmak için bir tuşa tıklayınız..");
                Console.ReadLine();
            }
        }
    }
}
