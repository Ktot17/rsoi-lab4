using GatewayBL.Enums;

namespace GatewayBL.Exceptions;

public class ServiceIsUnavailableException : Exception
{
    public ServiceIsUnavailableException(ServiceName serviceName) : 
        base($"{serviceName.ToString()} service is not responding. Please retry later.") { }
    
    public ServiceIsUnavailableException() { }

    public ServiceIsUnavailableException(string message) : base(message) { }

    public ServiceIsUnavailableException(string message, Exception innerException)
        : base(message, innerException) { }
}
