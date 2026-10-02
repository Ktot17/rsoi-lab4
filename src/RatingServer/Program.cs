using Microsoft.EntityFrameworkCore;
using RatingBL.InputPorts;
using RatingBL.Managers;
using RatingBL.OutputPorts;
using RatingDB.Postgres;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<PostgresDbContext>(options =>
    options.UseNpgsql(builder.Configuration["ConnectionStrings:Postgres"]!));
builder.Services.AddScoped<IRatingRepository, RatingRepository>();

builder.Services.AddScoped<IRatingManager, RatingManager>();

var app = builder.Build();

app.MapControllers();
app.MapGet("manage/health", () => Results.Ok("Healthy"));

app.Run();