using System;
using System.Net;
using System.Net.Mail;

public class Program
{
    public static bool EmailSend(string email, MemoryStream memory, string fileName)
    {
        try
        {
            memory.Position = 0;
            System.Net.Mime.ContentType contentType = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);

            Attachment attachment = new Attachment(memory, contentType);
            attachment.ContentDisposition.FileName = $"{fileName}.pdf";

            MailMessage mailMessage = new MailMessage();
            SmtpClient smtpClient = new SmtpClient();

            mailMessage.From = new MailAddress("tiryaki52341903@gmail.com");
            mailMessage.To.Add(fileName);
            mailMessage.Subject = "Pdf Dosyası Hk.";
            mailMessage.Body = $"{fileName} isimli Word dosyanız, Pdf dosyasına çevirilmiştir. İlgili dosyayı ekte bulabilirsiniz.";
            mailMessage.IsBodyHtml = true;

            mailMessage.Attachments.Add(attachment);

            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;

            NetworkCredential networkCredential = new NetworkCredential("tiryaki52341903@gmail.com", "19031998beyto");
            smtpClient.Credentials = networkCredential;

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
}
