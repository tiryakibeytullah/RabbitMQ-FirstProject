using Microsoft.EntityFrameworkCore;
using RabbitMQWeb.Watermark.Models.Products;

namespace RabbitMQWeb.Watermark.Models.DbContext
{
    public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
    }
}
