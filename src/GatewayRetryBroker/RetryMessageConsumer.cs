using System.Text;
using GatewayBL.Enums;
using GatewayBL.Exceptions;
using GatewayBL.InputPorts;
using GatewayRetryBroker.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GatewayRetryBroker;

public class RetryMessageConsumer : BackgroundService
{
    private readonly IChannel _channel;
    private readonly RabbitMqConfig _config;
    private readonly ILibraryHttpClient _libraryHttpClient;
    private readonly IRatingHttpClient _ratingHttpClient;
    private readonly IReservationHttpClient _reservationHttpClient;

    public RetryMessageConsumer(
        IConnection connection,
        IOptions<RabbitMqConfig> options,
        ILibraryHttpClient libraryHttpClient,
        IRatingHttpClient ratingHttpClient,
        IReservationHttpClient reservationHttpClient
    )
    {
        _channel = connection.CreateChannelAsync().Result;
        RabbitTopology.Declare(_channel, options).Wait();
        _config = options.Value;
        _libraryHttpClient = libraryHttpClient;
        _ratingHttpClient = ratingHttpClient;
        _reservationHttpClient = reservationHttpClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, ea) => await HandleAsync(ea, stoppingToken);

        await _channel.BasicConsumeAsync(
            queue: _config.MainQueue,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);
        
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleAsync(BasicDeliverEventArgs ea, CancellationToken ct)
    {
        var requestType = ReadRequestType(ea.BasicProperties);
        var json = Encoding.UTF8.GetString(ea.Body.Span);

        try
        {
            switch (requestType)
            {
                case RequestType.RevertTakeBook:
                    var revertRequest = JsonConvert.DeserializeObject<RevertTakeBookRequestJson>(json) ??
                                  throw new InvalidOperationException();
                    await _reservationHttpClient.RevertTakeBookAsync(revertRequest.ReservationUid);
                    break;
                case RequestType.ChangeAvailableCount:
                    var changeRequest = JsonConvert.DeserializeObject<ChangeAvailableCountRequestJson>(json) ??
                                  throw new InvalidOperationException();
                    await _libraryHttpClient.ChangeAvailableCountAsync(
                        changeRequest.LibraryUid, changeRequest.BookUid, changeRequest.Count);
                    break;
                case RequestType.UpdateRating:
                    var updateRequest = JsonConvert.DeserializeObject<UpdateRatingRequestJson>(json) ??
                                        throw new InvalidOperationException();
                    await _ratingHttpClient.UpdateUserRatingAsync(updateRequest.Username, updateRequest.Stars);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        catch (ServiceIsUnavailableException)
        {
            await _channel.BasicPublishAsync(
                exchange: _config.RetryExchange,
                routingKey: _config.RoutingKey,
                mandatory: false,
                basicProperties: new BasicProperties(ea.BasicProperties),
                body: ea.Body,
                cancellationToken: ct);
        }
        await _channel.BasicAckAsync(ea.DeliveryTag, false, ct);
    }

    private static RequestType ReadRequestType(IReadOnlyBasicProperties props)
    {
        if (props.Headers is null ||
            !props.Headers.TryGetValue("request-type", out var raw) || raw is null)
        {
            throw new InvalidOperationException("Сообщение без заголовка 'request-type'");
        }

        return (RequestType)raw;
    }
}