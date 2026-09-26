namespace GatewayServer.Models;

public record BookReservationResponse(Guid ReservationUid, string Status,
    string StartDate, string TillDate, BookInfo Book, LibraryResponse Library);