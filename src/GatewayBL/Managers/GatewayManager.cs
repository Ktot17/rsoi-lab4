using GatewayBL.Enums;
using GatewayBL.Exceptions;
using GatewayBL.InputPorts;
using GatewayBL.Models;
using GatewayBL.OutputPorts;

namespace GatewayBL.Managers;

public class GatewayManager(
    ILibraryHttpClient libraryHttpClient,
    IRatingHttpClient ratingHttpClient,
    IReservationHttpClient reservationHttpClient,
    IRetryMessageProducer retryMessageProducer) : IGatewayManager
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

        try
        {
            var (libraries, books) = await libraryHttpClient.GetLibrariesAndBooksAsync(libraryUids, bookUids);

            var librariesById = libraries.ToDictionary(l => l.LibraryUid);
            var booksById = books.ToDictionary(b => b.BookUid);

            return reservations.Select(r => new FullInfo(r, booksById[r.BookUid], librariesById[r.LibraryUid]));
        }
        catch (ServiceIsUnavailableException)
        {
            return reservations.Select(r => new FullInfo(r, null, null));
        }
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

        try
        {
            await libraryHttpClient.ChangeAvailableCountAsync(libraryUid, bookUid, -1);
        }
        catch (ServiceIsUnavailableException)
        {
            try
            {
                await reservationHttpClient.RevertTakeBookAsync(reservation.ReservationUid);
            }
            catch (ServiceIsUnavailableException)
            {
                await retryMessageProducer.SendAsync(RequestType.RevertTakeBook, new RevertTakeBookRequest(reservation.ReservationUid));
            }
            throw;
        }

        try
        {
            var (libraries, books) =
                await libraryHttpClient.GetLibrariesAndBooksAsync([libraryUid], [bookUid]);

            return new FullInfo(reservation, books.First(), libraries.First(), rating);
        }
        catch (ServiceIsUnavailableException)
        {
            return new FullInfo(reservation, null, null, rating);
        }
    }

    public async Task ReturnBookAsync(string username, Guid reservationUid, Condition condition, DateTime returnDate)
    {
        var reservation = await reservationHttpClient.ReturnBookAsync(reservationUid, returnDate);
        try
        {
            await libraryHttpClient.ChangeAvailableCountAsync(reservation.LibraryUid, reservation.BookUid, 1);
        }
        catch (ServiceIsUnavailableException)
        {
            await retryMessageProducer.SendAsync(RequestType.ChangeAvailableCount,
                new ChangeAvailableCountRequest(reservation.LibraryUid, reservation.BookUid, 1));
        }
        
        var conditionMismatch = condition is Condition.BAD;
        var isExpired = reservation.Status is Status.EXPIRED;

        var count = (conditionMismatch, isExpired) switch
        {
            (true, true) => 2,
            (true, false) or (false, true) => 1,
            _ => 0
        };

        var ratingChange = count == 0 ? 1 : -10 * count;
        try
        {
            await ratingHttpClient.UpdateUserRatingAsync(username, ratingChange);
        }
        catch (ServiceIsUnavailableException)
        {
            await retryMessageProducer.SendAsync(RequestType.UpdateRating, new UpdateRatingRequest(username, ratingChange));
        }
    }

    public async Task<int> GetUserRatingAsync(string username) => 
        await ratingHttpClient.GetUserRatingAsync(username);
}
