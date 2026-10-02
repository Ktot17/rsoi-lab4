namespace ReservationServer.Models;

public record TakeBookRequest(Guid LibraryUid, Guid BookUid, DateTime TillDate);

public record ReturnBookRequest(DateTime Date);