using System.Text;
using GatewayBL.Enums;
using GatewayBL.InputPorts;
using GatewayBL.Models;
using GatewayRetryBroker.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace GatewayRetryBroker;

public class RetryMessageProducer : IRetryMessageProducer, IDisposable
{
    private readonly IChannel _channel;
    private readonly RabbitMqConfig _config;

    public RetryMessageProducer(
        IConnection connection,
        IOptions<RabbitMqConfig> options)
    {
        _channel = connection.CreateChannelAsync().Result;
        RabbitTopology.Declare(_channel, options).Wait();
        _config = options.Value;
    }
    
    public async Task SendAsync(RequestType requestType, object request)
    {
        string json;
        switch (requestType)
        {
            case RequestType.RevertTakeBook:
                var revertTakeBookRequest = (RevertTakeBookRequest)request;
                json = JsonConvert.SerializeObject(new RevertTakeBookRequestJson(revertTakeBookRequest.ReservationUid));
                break;
            case RequestType.ChangeAvailableCount:
                var changeAvailableCountRequest = (ChangeAvailableCountRequest)request;
                json = JsonConvert.SerializeObject(
                    new ChangeAvailableCountRequestJson(changeAvailableCountRequest.LibraryUid,
                        changeAvailableCountRequest.BookUid, changeAvailableCountRequest.Count));
                break;
            case RequestType.UpdateRating:
                var updateRatingRequest = (UpdateRatingRequest)request;
                json = JsonConvert.SerializeObject(new UpdateRatingRequestJson(updateRatingRequest.Username, updateRatingRequest.Stars));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null);
        }
        
        var body = Encoding.UTF8.GetBytes(json);
        var props = new BasicProperties
        {
            Persistent = true,
            Headers = new Dictionary<string, object?>
            {
                ["request-type"] = (int)requestType,
            }
        };

        await _channel.BasicPublishAsync(
            exchange: _config.MainExchange,
            routingKey: _config.RoutingKey,
            mandatory: false,
            basicProperties: props,
            body: body
            );
    }
    
    public void Dispose() => _channel.Dispose();
}