namespace GatewayServer.Models;

public record BookReservationWithUidsResponse(Guid ReservationUid, string Status,
    string StartDate, string TillDate, Guid BookUid, Guid LibraryUid);
    