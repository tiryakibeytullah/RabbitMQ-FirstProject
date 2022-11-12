namespace RabbitMQWeb.WorkToPdf.Models
{
    public class WordToPdf
    {
        public string Email { get; set; }
        public IFormFile WordFile { get; set; }
    }
}
