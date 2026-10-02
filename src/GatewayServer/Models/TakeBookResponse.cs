namespace GatewayServer.Models;

public record TakeBookResponse(
    Guid ReservationUid,
    string Status,
    string StartDate,
    string TillDate,
    BookInfo Book,
    LibraryResponse Library,
    UserRatingResponse Rating);
    