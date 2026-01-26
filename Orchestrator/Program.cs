
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
                client.BaseAddress = new Uri("https://marconnesapi-bbezaba2hph7asad.westeurope-01.azurewebsites.net/");
            });
            builder.Services.AddHttpClient("CampingAPI", client =>
            {
                client.BaseAddress = new Uri("https://campingef-api-bnfxe6egdfhac5ck.westeurope-01.azurewebsites.net/");
            });
            builder.Services.AddHttpClient("GiteAPI", client =>
            {
                client.BaseAddress = new Uri("https://gite-api-01-c5b5fhb0ddadb9d6.westeurope-01.azurewebsites.net/");
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
