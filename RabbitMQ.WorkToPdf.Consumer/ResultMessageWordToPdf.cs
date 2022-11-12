namespace RabbitMQ.WorkToPdf.Consumer
{
    public class ResultMessageWordToPdf
    {
        public byte[] WordByte { get; set; }
        public string Email { get; set; }
        public string FileName { get; set; }
    }
}
