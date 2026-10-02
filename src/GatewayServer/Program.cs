using GatewayBL.InputPorts;
using GatewayBL.Managers;
using GatewayBL.OutputPorts;
using GatewayHttp.Clients;
using GatewayRetryBroker;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpClient<ILibraryHttpClient, LibraryHttpClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetSection("ServiceUrls")["LibraryServiceUrl"]!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).AddStandardResilienceHandler();
builder.Services.AddHttpClient<IRatingHttpClient, RatingHttpClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetSection("ServiceUrls")["RatingServiceUrl"]!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).AddStandardResilienceHandler();
builder.Services.AddHttpClient<IReservationHttpClient, ReservationHttpClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetSection("ServiceUrls")["ReservationServiceUrl"]!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).AddStandardResilienceHandler();

builder.Services.Configure<RabbitMqConfig>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddSingleton<IConnection>(sp =>
{
    var opt = sp.GetRequiredService<IOptions<RabbitMqConfig>>().Value;
    var factory = new ConnectionFactory
    {
        HostName = opt.Host,
        Port = opt.Port,
        UserName = opt.Username,
        Password = opt.Password,
        AutomaticRecoveryEnabled = true
    };
    return factory.CreateConnectionAsync().GetAwaiter().GetResult();
});

builder.Services.AddSingleton<IRetryMessageProducer, RetryMessageProducer>();
builder.Services.AddHostedService<RetryMessageConsumer>();

builder.Services.AddScoped<IGatewayManager, GatewayManager>();

var app = builder.Build();

app.MapControllers();
app.MapGet("manage/health", () => Results.Ok("Healthy"));

await app.RunAsync();
