
using Microsoft.EntityFrameworkCore;
using ReservationSystem.API.Services;
using ReservationSystem.API.Worker;
using ReservationSystem.Application.Handler.CreateReservation;
using ReservationSystem.Application.Interfaces.Admission;
using ReservationSystem.Application.Interfaces.Cache;
using ReservationSystem.Application.Interfaces.Metric;
using ReservationSystem.Application.Interfaces.Randomizer;
using ReservationSystem.Application.Interfaces.Repository;
using ReservationSystem.Application.Interfaces.UnitOfWork;
using ReservationSystem.Infrastructure.Admission;
using ReservationSystem.Infrastructure.Connection;
using ReservationSystem.Infrastructure.Observability;
using ReservationSystem.Infrastructure.Persistence;
using ReservationSystem.Infrastructure.Persistence.Repository;
using StackExchange.Redis;

namespace ReservationSystem.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
            builder.Services.AddScoped<ISeatCategoryRepository, SeatCategoryRepository>();
            builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

            builder.Services.AddScoped<ISeatCache, RedisSeatCache>();
            builder.Services.AddScoped<ISeatRequestGate, SeatRequestGate>();
            builder.Services.AddScoped<IRandomizer, Randomizer>();

            builder.Services.AddSingleton<IReservationMetric, InMemoryReservationMetric>();

            //Conn strings
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

            //Conn for redis
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configuration = builder.Configuration.GetConnectionString("Redis");

                //If redis isnt up/connected, abort connection
                var options = ConfigurationOptions.Parse(configuration);
                options.AbortOnConnectFail = false;

                return ConnectionMultiplexer.Connect(options);
            });

            //builder.Services.AddHostedService<ExpirationWorker>();
            builder.Services.AddHostedService<ReservationMetricSnapshotService>();

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CreateReservationCommand).Assembly);
            });

            var app = builder.Build();

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
