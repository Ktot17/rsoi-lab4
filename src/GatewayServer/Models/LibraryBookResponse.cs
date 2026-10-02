namespace GatewayServer.Models;

public record LibraryBookResponse(Guid BookUid, string Name, string? Author, string? Genre, string Condition, int AvailableCount);