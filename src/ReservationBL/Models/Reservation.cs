using ReservationBL.Enums;

namespace ReservationBL.Models;

public record Reservation(Guid ReservationUid, string Username,
    Guid BookUid, Guid LibraryUid, Status Status, DateTime StartDate, DateTime TillDate);