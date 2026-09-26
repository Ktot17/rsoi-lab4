namespace GatewayServer.Models;

public record BookInfo(Guid BookUid, string Name, string? Author, string? Genre);