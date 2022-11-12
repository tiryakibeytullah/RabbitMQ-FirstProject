using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQWeb.WorkToPdf.Models;
using System.Diagnostics;
using System.Text;

namespace RabbitMQWeb.WorkToPdf.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult ConvertWordToPdf()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ConvertWordToPdf(WordToPdf wordToPdf)
        {
            try
            {
                var factory = new ConnectionFactory();
                var queueName = "file-queue";
                var exchangeName = "convert-exchange";
                var routingKeyName = "word-to-pdf";
                factory.Uri = new Uri(_configuration.GetConnectionString("RabbitMQ"));

                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false, null); 
                        channel.QueueDeclare(queueName, true, false, false, null);
                        channel.QueueBind(queueName, exchangeName, routingKeyName);

                        ResultMessageWorkToPdf resultMessageWorkToPdf = new();
                        using (MemoryStream stream = new MemoryStream())
                        {
                            //Hafızaya kopyala
                            wordToPdf.WordFile.CopyTo(stream);
                            resultMessageWorkToPdf.WordByte = stream.ToArray();
                        }

                        resultMessageWorkToPdf.Email = wordToPdf.Email;
                        resultMessageWorkToPdf.FileName = Path.GetFileNameWithoutExtension(wordToPdf.WordFile.FileName);

                        string serializeMessage = JsonConvert.SerializeObject(resultMessageWorkToPdf);
                        var byteResultMessage = Encoding.UTF8.GetBytes(serializeMessage);

                        //Gelen mesajlar rabbitMq restart olsa dahil kalıcı olucak
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        channel.BasicPublish(exchangeName, routingKeyName, properties, byteResultMessage);

                        ViewBag.Result = "İlgili Word dosyanız Pdf dosyasına dönüştürme işleminden sonra, Mail adresinize gönderilecektir.";

                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}