namespace GatewayServer.Models;

public record TakeBookRequest(Guid BookUid, Guid LibraryUid, DateTime TillDate);