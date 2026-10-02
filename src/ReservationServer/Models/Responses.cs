using ReservationBL.Enums;
using ReservationBL.Models;

namespace ReservationServer.Models;

public record ReservationResponse(
    Guid ReservationUid,
    Status Status,
    DateTime StartDate,
    DateTime TillDate,
    Guid BookUid,
    Guid LibraryUid)
{
    public ReservationResponse(Reservation reservation) :
        this(reservation.ReservationUid, reservation.Status, reservation.StartDate, reservation.TillDate,
            reservation.BookUid, reservation.LibraryUid) {}
}