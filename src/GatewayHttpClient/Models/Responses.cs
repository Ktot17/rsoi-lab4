using GatewayBL.Enums;
using GatewayBL.Models;

namespace GatewayHttp.Models;

public record LibraryResponse(Guid LibraryUid, string Name, string Address, string City)
{
    public Library ToBlModel() => new Library(LibraryUid, Name, Address, City);
}

public record LibraryPaginationResponse(int Page, int PageSize, int TotalElements, IEnumerable<LibraryResponse> Items);

public record LibrariesAndBooksResponse(IEnumerable<Library> Libraries, IEnumerable<Book> Books);

public record LibraryBookResponse(
    Guid BookUid,
    string Name,
    string? Author,
    string? Genre,
    Condition Condition,
    int AvailableCount)
{
    public Book ToBlModel() => new Book(BookUid, Name, Author, Genre, Condition, AvailableCount);
}

public record LibraryBookPaginationResponse(int Page, int PageSize, int TotalElements, IEnumerable<LibraryBookResponse> Items);

public record UserRatingResponse(int Stars);

public record ReservationResponse(
    Guid ReservationUid,
    Status Status,
    DateTime StartDate,
    DateTime TillDate,
    Guid BookUid,
    Guid LibraryUid)
{
    public Reservation ToBlModel() => new Reservation(ReservationUid, Status, StartDate, TillDate, BookUid, LibraryUid);
}