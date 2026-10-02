namespace GatewayServer.Models;

public record TakeBookWithUidsResponse(
    Guid ReservationUid,
    string Status,
    string StartDate,
    string TillDate,
    Guid BookUid,
    Guid LibraryUid,
    UserRatingResponse Rating);