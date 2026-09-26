using GatewayBL.Enums;

namespace GatewayBL.Models;

public record Reservation(Guid ReservationUid, Status Status, DateTime StartDate, DateTime TillDate, Guid BookUid, Guid LibraryUid);