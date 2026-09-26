using GatewayBL.InputPorts;
using GatewayBL.Managers;
using GatewayBL.OutputPorts;
using GatewayHttp.Clients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpClient<ILibraryHttpClient, LibraryHttpClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetSection("ServiceUrls")["LibraryServiceUrl"]!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddHttpClient<IRatingHttpClient, RatingHttpClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetSection("ServiceUrls")["RatingServiceUrl"]!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddHttpClient<IReservationHttpClient, ReservationHttpClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetSection("ServiceUrls")["ReservationServiceUrl"]!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddScoped<IGatewayManager, GatewayManager>();

var app = builder.Build();

app.MapControllers();
app.MapGet("manage/health", () => Results.Ok("Healthy"));

app.Run();