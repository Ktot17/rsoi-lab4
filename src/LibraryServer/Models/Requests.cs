namespace LibraryServer.Models;

public record LibrariesAndBooksRequest(IEnumerable<Guid> LibraryUids, IEnumerable<Guid> BookUids);