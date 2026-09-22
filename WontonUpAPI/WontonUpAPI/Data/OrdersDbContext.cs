using Microsoft.EntityFrameworkCore;
using WontonUpAPI.Models;

namespace WontonUpAPI.Data
{
    public class OrdersDbContext : DbContext
    {
        public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options)
        {
        }
        public DbSet<Order> Orders { get; set; } = null!;
    }
}
