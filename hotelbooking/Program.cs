using hotelbooking.Services;
using hotelbooking.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
namespace hotelbooking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(
                        new JsonStringEnumConverter());
                });

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySQL(
                    builder.Configuration.GetConnectionString("DefaultConnection")!                  
                ));
            builder.Services.AddScoped<CustomerService>();
            builder.Services.AddScoped<RoomService>();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();
            app.UseExceptionHandler("/error");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
