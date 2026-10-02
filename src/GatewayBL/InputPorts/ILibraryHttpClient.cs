using GatewayBL.Models;

namespace GatewayBL.InputPorts;

public interface ILibraryHttpClient
{
    public Task<(int, IEnumerable<Library>)> GetLibraries(string city, int page, int size);
    public Task<(int, IEnumerable<Book>)> GetBooks(Guid libraryUid, int page, int size, bool showAll);
    public Task<(IEnumerable<Library>, IEnumerable<Book>)> GetLibrariesAndBooksAsync(
        IEnumerable<Guid> libraryUids, IEnumerable<Guid> bookUids);
    public Task ChangeAvailableCountAsync(Guid libraryUid, Guid bookUid, int count);
}