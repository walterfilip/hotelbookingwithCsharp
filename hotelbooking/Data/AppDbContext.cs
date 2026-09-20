using hotelbooking.Models;
using Microsoft.EntityFrameworkCore;

namespace hotelbooking.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
        }
        public DbSet<Customer> Customers { get; set; }
    }
}
