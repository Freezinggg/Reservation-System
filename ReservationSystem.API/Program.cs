
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Application.Handler.CreateReservation;
using ReservationSystem.Application.Interfaces.Repository;
using ReservationSystem.Application.Interfaces.UnitOfWork;
using ReservationSystem.Infrastructure.Persistence;
using ReservationSystem.Infrastructure.Persistence.Repository;

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

            //Conn strings
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

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
