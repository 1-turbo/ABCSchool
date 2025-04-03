
using Infrastructure;

namespace WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers(); 

            builder.Services.AddInfrastructureServices(builder.Configuration);

            builder.Services.AddJwtAuthentication(builder.Services.GetJwtSettings(builder.Configuration));

            var app = builder.Build();

            // Database Seeder
            await app.Services.AddDatabaseInitializeAsync();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseInfrastructure();

            app.MapControllers();

            app.Run();
        }
    }
}
