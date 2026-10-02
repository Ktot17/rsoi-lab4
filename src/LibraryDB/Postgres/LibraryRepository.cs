using LibraryBL.InputPorts;
using LibraryBL.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryDB.Postgres;

public class LibraryRepository(PostgresDbContext context) : ILibraryRepository
{
    public async Task<(int, IEnumerable<Library>)> GetLibraries(string city, int page, int size)
    {
        var query = context.Libraries.Where(l => l.City == city);
        
        var totalCount = await query.CountAsync();
        
        return (totalCount, await query.OrderBy(l => l.Id)
            .Skip((page - 1) * size).Take(size)
            .Select(l => l.ToBlModel()).ToListAsync());
    }

    public async Task<IEnumerable<Library>> GetLibraries(IEnumerable<Guid> libraryUids) => 
        await context.Libraries.Where(l => libraryUids.Contains(l.LibraryUid))
            .Select(l => l.ToBlModel()).ToListAsync();
}