namespace GatewayHttp.Models;

public record LibrariesAndBooksRequest(IEnumerable<Guid> LibraryUids, IEnumerable<Guid> BookUids);

public record TakeBookRequest(Guid LibraryUid, Guid BookUid, DateTime TillDate);

public record ReturnBookRequest(DateTime Date);