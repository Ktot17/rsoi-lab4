using GatewayBL.Enums;

namespace GatewayBL.InputPorts;

public interface IRetryMessageProducer
{
    public Task SendAsync(RequestType requestType, object request);
}