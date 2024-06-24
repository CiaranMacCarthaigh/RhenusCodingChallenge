using RhenusCodingChallenge.Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;
using RhenusCodingChallenge.Application.Services;
using RhenusCodingChallenge.Services;
using RhenusCodingChallenge.Application.Repositories;
using RhenusCodingChallenge.Infrastructure.Database.Repositories;
using RhenusCodingChallenge.Application.Player.Commands.CreateNewPlayer;
using RhenusCodingChallenge.Application.Services.Game;
using RhenusCodingChallenge.WebApi.Middleware;
using NodaTime;

namespace RhenusCodingChallenge.WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateNewPlayerCommand).Assembly));

            builder.Services.AddScoped<IGuidProviderService, GuidProviderService>();
            builder.Services.AddScoped<IDomainEventRepository, DomainEventRepository>();
            builder.Services.AddScoped<IGameService, GameService>();
            builder.Services.AddSingleton<IClock>(SystemClock.Instance);
            builder.Services.AddSingleton<IRandomNumberGeneratorService, RandomNumberGeneratorService>();

            builder.Services.AddDbContext<EventStorageDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("EventStorageDbContext"),
                    x => x.UseNodaTime())
            );

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<EventStorageDbContext>();
                await context.Database.MigrateAsync();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.Run();
        }
    }
}
