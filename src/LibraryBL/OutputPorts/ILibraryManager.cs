using LibraryBL.Models;

namespace LibraryBL.OutputPorts;

public interface ILibraryManager
{
    public Task<(int, IEnumerable<Library>)> GetLibraries(string city, int page, int size);
    public Task<(int, IEnumerable<Book>)> GetBooks(Guid libraryUid, bool showAll, int page, int size);
    public Task<(IEnumerable<Library>, IEnumerable<Book>)> GetLibrariesAndBooks(IEnumerable<Guid> libraryUids,
        IEnumerable<Guid> bookUids);
    public Task ChangeAvailableCount(Guid libraryUid, Guid bookUid, int count);
}