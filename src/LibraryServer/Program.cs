using LibraryBL.InputPorts;
using LibraryBL.Managers;
using LibraryBL.OutputPorts;
using LibraryDB.Postgres;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<PostgresDbContext>(options =>
    options.UseNpgsql(builder.Configuration["ConnectionStrings:Postgres"]!));
builder.Services.AddScoped<ILibraryRepository, LibraryRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();

builder.Services.AddScoped<ILibraryManager, LibraryManager>();

var app = builder.Build();

app.MapControllers();
app.MapGet("manage/health", () => Results.Ok("Healthy"));

app.Run();