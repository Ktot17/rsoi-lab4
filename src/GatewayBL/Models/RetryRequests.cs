namespace GatewayBL.Models;

public record RevertTakeBookRequest(Guid ReservationUid);

public record ChangeAvailableCountRequest(Guid LibraryUid, Guid BookUid, int Count);

public record UpdateRatingRequest(string Username, int Stars);
