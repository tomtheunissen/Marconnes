
using Microsoft.Extensions.DependencyInjection;
using Orchestrator.ApiConnector;
using System;

namespace Orchestrator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
          

            // Add http API Camping & Hotel
            builder.Services.AddHttpClient("HotelAPI", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7164/");
            });
            builder.Services.AddHttpClient("CampingAPI", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7022/");
            });
            builder.Services.AddHttpClient("GiteAPI", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7198");
            });

            // Dependancy Injection
            builder.Services.AddScoped<OrchService>();

            //App builder
            var app = builder.Build();


            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();
            
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
