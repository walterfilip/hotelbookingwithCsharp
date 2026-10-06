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
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Room>()
                .Property(r => r.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Customer)
                .WithMany()
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Booking>()
                .Property(b => b.CheckIn)
                .HasConversion(
                    dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
                    dateTime => DateOnly.FromDateTime(dateTime))
                .HasColumnType("date");

            modelBuilder.Entity<Booking>()
                .Property(b => b.CheckOut)
                .HasConversion(
                    dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
                    dateTime => DateOnly.FromDateTime(dateTime))
                .HasColumnType("date");

            modelBuilder.Entity<Booking>()
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
    
}
