using LibraryBL.InputPorts;
using LibraryBL.Models;
using LibraryBL.OutputPorts;

namespace LibraryBL.Managers;

public class LibraryManager(ILibraryRepository libraryRepository, IBookRepository bookRepository) : ILibraryManager
{
    public async Task<(int, IEnumerable<Library>)> GetLibraries(string city, int page, int size) =>
        await libraryRepository.GetLibraries(city, page, size);
    
    public async Task<(int, IEnumerable<Book>)> GetBooks(Guid libraryUid, bool showAll, int page, int size) =>
        await bookRepository.GetBooks(libraryUid, showAll, page, size);

    public async Task<(IEnumerable<Library>, IEnumerable<Book>)> GetLibrariesAndBooks(IEnumerable<Guid> libraryUids,
        IEnumerable<Guid> bookUids) => 
        (await libraryRepository.GetLibraries(libraryUids), await bookRepository.GetBooks(bookUids));
    
    public async Task ChangeAvailableCount(Guid libraryUid, Guid bookUid, int count) => 
        await bookRepository.ChangeAvailableCount(libraryUid, bookUid, count);
}