using DemoWebApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DemoWebApi.Data;
using Microsoft.Build.Execution;

namespace DemoWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<BookContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("BookContext") ?? throw new InvalidOperationException("Connection string 'BookContext' not found.")));

            // Add services to the container.

            builder.Services.AddControllers();

            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:4200")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();

            IWebHostEnvironment _env = app.Services.GetRequiredService<IWebHostEnvironment>();

            app.UseCors(MyAllowSpecificOrigins);

            app.MapControllers();

            app.Run();
        }
    }
}