using Microsoft.EntityFrameworkCore;
using ReservationBL.InputPorts;
using ReservationBL.Managers;
using ReservationBL.OutputPorts;
using ReservationDB.Postgres;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<PostgresDbContext>(options =>
    options.UseNpgsql(builder.Configuration["ConnectionStrings:Postgres"]!));
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

builder.Services.AddScoped<IReservationManager, ReservationManager>();

var app = builder.Build();

app.MapControllers();
app.MapGet("manage/health", () => Results.Ok("Healthy"));

app.Run();
