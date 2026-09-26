using GatewayBL.Enums;
using GatewayBL.Exceptions;
using GatewayBL.InputPorts;
using GatewayBL.Models;
using GatewayBL.OutputPorts;

namespace GatewayBL.Managers;

public class GatewayManager(
    ILibraryHttpClient libraryHttpClient,
    IRatingHttpClient ratingHttpClient,
    IReservationHttpClient reservationHttpClient) : IGatewayManager
{
    public async Task<(int, IEnumerable<Library>)> GetLibrariesAsync(string city, int page, int size) => 
        await libraryHttpClient.GetLibraries(city, page, size);
    
    public async Task<(int, IEnumerable<Book>)> GetBooksAsync(Guid libraryUid, int page, int size, bool showAll) => 
        await libraryHttpClient.GetBooks(libraryUid, page, size, showAll);

    public async Task<IEnumerable<FullInfo>> GetReservationsAsync(string username)
    {
        var reservations = (await reservationHttpClient.GetReservationsAsync(username)).ToList();

        var libraryUids = reservations.Select(r => r.LibraryUid).Distinct().ToList();
        var bookUids = reservations.Select(r => r.BookUid).Distinct().ToList();

        var (libraries, books) = await libraryHttpClient.GetLibrariesAndBooksAsync(libraryUids, bookUids);

        var librariesById = libraries.ToDictionary(l => l.LibraryUid);
        var booksById = books.ToDictionary(b => b.BookUid);
        
        return reservations.Select(r => new FullInfo(r, booksById[r.BookUid], librariesById[r.LibraryUid]));
    }

    public async Task<FullInfo> TakeBookAsync(string username, Guid libraryUid, Guid bookUid, DateTime tillDate)
    {
        var rentedCountTask = reservationHttpClient.GetReservationCountAsync(username);
        var ratingTask = ratingHttpClient.GetUserRatingAsync(username);
        
        await Task.WhenAll(rentedCountTask, ratingTask);
        
        var rentedCount = await rentedCountTask;
        var rating = await ratingTask;

        if (rentedCount >= rating)
            throw new TooManyRentedBooksException();
        
        var reservation = await reservationHttpClient.TakeBookAsync(username, libraryUid, bookUid, tillDate);
        await libraryHttpClient.ChangeAvailableCountAsync(libraryUid, bookUid, -1);
        var (libraries, books) = 
            await libraryHttpClient.GetLibrariesAndBooksAsync([libraryUid], [bookUid]);
        
        return new FullInfo(reservation, books.First(), libraries.First(), rating);
    }

    public async Task ReturnBookAsync(string username, Guid reservationUid, Condition condition, DateTime returnDate)
    {
        var reservation = await reservationHttpClient.ReturnBookAsync(reservationUid, returnDate);
        await libraryHttpClient.ChangeAvailableCountAsync(reservation.LibraryUid, reservation.BookUid, 1);
        var (_, books) = await libraryHttpClient.GetLibrariesAndBooksAsync([], [reservation.BookUid]);
        var book = books.First();
        var count = 0;

        if (book.Condition != condition && reservation.Status is Status.EXPIRED)
            count = 2;
        else if (book.Condition != condition || reservation.Status is Status.EXPIRED)
            count = 1;

        var ratingChange = count == 0 ? 1 : -10 * count;
        await ratingHttpClient.UpdateUserRatingAsync(username, ratingChange);
    }

    public async Task<int> GetUserRatingAsync(string username) => 
        await ratingHttpClient.GetUserRatingAsync(username);
}