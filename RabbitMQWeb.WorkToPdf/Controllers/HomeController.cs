using Microsoft.AspNetCore.Mvc;
using RabbitMQWeb.WorkToPdf.Models;
using System.Diagnostics;

namespace RabbitMQWeb.WorkToPdf.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
            return View();
        }
    }
}