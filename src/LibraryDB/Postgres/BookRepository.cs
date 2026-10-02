using LibraryBL.InputPorts;
using LibraryBL.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryDB.Postgres;

public class BookRepository(PostgresDbContext context) : IBookRepository
{
    public async Task<(int, IEnumerable<Book>)> GetBooks(Guid libraryUid, bool showAll, int page, int size)
    {
        var query = context.Books
            .Where(b => context.LibraryBooks
                .Any(lb => lb.LibraryId ==
                           context.Libraries.Where(l => l.LibraryUid == libraryUid)
                               .Select(l => l.Id).FirstOrDefault()
                           && (showAll || lb.AvailableCount != 0) && lb.BookId == b.Id));
        
        var totalCount = await query.CountAsync();

        return (totalCount, await query.OrderBy(b => b.Id).Skip((page - 1) * size).Take(size)
            .Select(b => new
            {
                Book = b,
                AvailableCount = context.LibraryBooks
                    .Where(lb => lb.BookId == b.Id).Select(lb => lb.AvailableCount).FirstOrDefault()
            }).Select(x => x.Book.ToBlModel(x.AvailableCount)).ToListAsync());
    }
    
    public async Task<IEnumerable<Book>> GetBooks(IEnumerable<Guid> bookUids) => 
        await context.Books.Where(b => bookUids.Contains(b.BookUid))
            .Select(b => new
            {
                Book = b,
                AvailableCount = context.LibraryBooks
                    .Where(lb => lb.BookId == b.Id).Select(lb => lb.AvailableCount).FirstOrDefault()
            })
            .Select(x => x.Book.ToBlModel(x.AvailableCount)).ToListAsync();

    public async Task ChangeAvailableCount(Guid libraryUid, Guid bookUid, int count)
    {
        var libraryBook = await context.LibraryBooks
            .Where(lb => lb.LibraryId == 
                         context.Libraries.Where(l => l.LibraryUid == libraryUid)
                             .Select(l => l.Id).FirstOrDefault()
                         && lb.BookId == 
                         context.Books.Where(b => b.BookUid == bookUid).Select(b => b.Id).FirstOrDefault())
            .FirstOrDefaultAsync() ?? throw new KeyNotFoundException();
        libraryBook.AvailableCount += count;
        context.LibraryBooks.Update(libraryBook);
        await context.SaveChangesAsync();
    }
}