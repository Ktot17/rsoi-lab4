using LibraryBL.Models;

namespace LibraryBL.InputPorts;

public interface ILibraryRepository
{
    public Task<(int, IEnumerable<Library>)> GetLibraries(string city, int page, int size);
    public Task<IEnumerable<Library>> GetLibraries(IEnumerable<Guid> libraryUids);
}