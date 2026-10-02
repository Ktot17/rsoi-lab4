using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace GatewayRetryBroker;

public static class RabbitTopology
{
    public static async Task Declare(IChannel channel, IOptions<RabbitMqConfig> options)
    {
        var config = options.Value;
        
        await channel.ExchangeDeclareAsync(config.MainExchange, ExchangeType.Direct, durable: true);
        await channel.QueueDeclareAsync(config.MainQueue, durable: true, exclusive: false, autoDelete: false);
        await channel.QueueBindAsync(config.MainQueue, config.MainExchange, config.RoutingKey);

        var retryArgs = new Dictionary<string, object?>
        {
            { "x-dead-letter-exchange", config.MainExchange },
            { "x-dead-letter-routing-key", config.RoutingKey },
            { "x-message-ttl", 10_000 }
        };

        await channel.ExchangeDeclareAsync(config.RetryExchange, ExchangeType.Direct, durable: true);
        await channel.QueueDeclareAsync(config.RetryQueue, durable: true, exclusive: false, autoDelete: false,
            arguments: retryArgs);
        await channel.QueueBindAsync(config.RetryQueue, config.RetryExchange, config.RoutingKey);
    }
}