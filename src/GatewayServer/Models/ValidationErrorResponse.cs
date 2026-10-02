namespace GatewayServer.Models;

public record ValidationErrorResponse(string Message, IEnumerable<ErrorDescription> Errors);