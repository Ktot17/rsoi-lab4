namespace GatewayRetryBroker;

public class RabbitMqConfig
{
    public string Host { get; init; } = "localhost";
    public int Port { get; init; } = 5672;
    public string Username { get; init; } = "guest";
    public string Password { get; init; } = "guest";
    public string MainExchange { get; init; } = "main.exchange";
    public string MainQueue { get; init; } = "main.queue";
    public string RetryExchange { get; init; } = "retry.exchange";
    public string RetryQueue { get; init; } = "retry.queue";
    public string RoutingKey { get; init; } = "routing.key";
}
