using LibraryBL.Models;

namespace LibraryBL.InputPorts;

public interface IBookRepository
{
    public Task<(int, IEnumerable<Book>)> GetBooks(Guid libraryUid, bool showAll, int page, int size);
    public Task<IEnumerable<Book>> GetBooks(IEnumerable<Guid> bookUids);
    public Task ChangeAvailableCount(Guid libraryUid, Guid bookUid, int count);
}