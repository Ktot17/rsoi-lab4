using LibraryBL.Enums;
using LibraryBL.Models;

namespace LibraryServer.Models;

public record LibraryResponse(Guid LibraryUid, string Name, string Address, string City)
{
    public LibraryResponse(Library library) : this(library.LibraryUid, library.Name, library.Address, library.City) {}
}

public record LibraryPaginationResponse(int Page, int PageSize, int TotalElements, List<LibraryResponse> Items);

public record LibrariesAndBooksResponse(IEnumerable<Library> Libraries, IEnumerable<Book> Books);

public record LibraryBookResponse(
    Guid BookUid,
    string Name,
    string? Author,
    string? Genre,
    Condition Condition,
    int AvailableCount)
{
    public LibraryBookResponse(Book book) : 
        this(book.BookUid, book.Name, book.Author, book.Genre, book.Condition, book.AvailableCount) {}
}

public record LibraryBookPaginationResponse(int Page, int PageSize, int TotalElements, List<LibraryBookResponse> Items);