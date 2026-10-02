using System.ComponentModel.DataAnnotations;
using AutoFixture.Xunit2;
using GatewayBL.Enums;
using GatewayBL.InputPorts;
using GatewayBL.Managers;
using GatewayBL.Models;
using Moq;

namespace GatewayUnitTests;

public class UnitTestsBl
{
    private readonly Mock<ILibraryHttpClient> _libraryHttpClientMock = new();
    private readonly Mock<IRatingHttpClient> _ratingHttpClientMock = new();
    private readonly Mock<IReservationHttpClient> _reservationHttpClientMock = new();
    private readonly GatewayManager _gatewayManager;

    public UnitTestsBl()
    {
        _gatewayManager = new GatewayManager(_libraryHttpClientMock.Object, _ratingHttpClientMock.Object, _reservationHttpClientMock.Object);
    }
    
    [Theory, AutoData]
    public async Task GetLibrariesTest(string city, int page, int size)
    {
        _libraryHttpClientMock.Setup(l => l.GetLibraries(city, page, size)).ReturnsAsync((1,
            [new Library(Guid.NewGuid(), "name", "address", city)]));
        
        var res = await _gatewayManager.GetLibrariesAsync(city, page, size);
        
        Assert.Equal(1, res.Item1);
        Assert.Single(res.Item2);
        Assert.Equal(city, res.Item2.First().City);
        _libraryHttpClientMock.Verify(l => l.GetLibraries(city, page, size), Times.Once);
    }

    [Theory, AutoData]
    public async Task GetBooksTest(Guid libraryUid, int page, int size)
    {
        _libraryHttpClientMock.Setup(l => l.GetBooks(libraryUid, page, size, true))
            .ReturnsAsync((1, [new Book(Guid.NewGuid(), "name", "author", "genre", Condition.EXCELLENT, 1)]));
        
        var res = await _gatewayManager.GetBooksAsync(libraryUid, page, size, true);
        
        Assert.Equal(1, res.Item1);
        Assert.Single(res.Item2);
        _libraryHttpClientMock.Verify(l => l.GetBooks(libraryUid, page, size, true), Times.Once);
    }

    [Theory, AutoData]
    public async Task GetReservationsTest(string username, Guid reservationGuid, Guid libraryUid, Guid bookUid)
    {
        _reservationHttpClientMock.Setup(r => r.GetReservationsAsync(username))
            .ReturnsAsync([new Reservation(reservationGuid, Status.RENTED, DateTime.Now, DateTime.Now, bookUid, libraryUid)]);
        _libraryHttpClientMock.Setup(l =>
                l.GetLibrariesAndBooksAsync(new List<Guid> { libraryUid }, new List<Guid> { bookUid }))
            .ReturnsAsync(([new Library(libraryUid, "name", "address", "city")],
                [new Book(bookUid, "name", "author", "genre", Condition.EXCELLENT, 1)]));
        
        var res = (await _gatewayManager.GetReservationsAsync(username)).ToList();

        Assert.Single(res);
        Assert.Equal(reservationGuid, res[0].Reservation.ReservationUid);
        Assert.Equal(bookUid, res[0].Book.BookUid);
        Assert.Equal(libraryUid, res[0].Library.LibraryUid);
        _reservationHttpClientMock.Verify(l => l.GetReservationsAsync(username), Times.Once);
        _libraryHttpClientMock.Verify(l => 
            l.GetLibrariesAndBooksAsync(new List<Guid> { libraryUid }, new List<Guid> { bookUid }), Times.Once);
    }

    [Theory, AutoData]
    public async Task TakeBooksTest(string username, Guid reservationUid, Guid libraryUid, Guid bookUid)
    {
        _reservationHttpClientMock.Setup(r => r.GetReservationCountAsync(username))
            .ReturnsAsync(1);
        _ratingHttpClientMock.Setup(r => r.GetUserRatingAsync(username))
            .ReturnsAsync(2);
        _reservationHttpClientMock.Setup(r => 
                r.TakeBookAsync(username, libraryUid, bookUid, It.IsAny<DateTime>()))
            .ReturnsAsync(new Reservation(reservationUid, Status.RENTED, DateTime.Now, DateTime.Now, bookUid, libraryUid));
        _libraryHttpClientMock.Setup(l =>
            l.GetLibrariesAndBooksAsync(new List<Guid> { libraryUid }, new List<Guid> { bookUid }))
            .ReturnsAsync(([new Library(libraryUid, "name", "address", "city")],
                [new Book(bookUid, "name", "author", "genre", Condition.EXCELLENT, 1)]));
        
        var res = await _gatewayManager.TakeBookAsync(username, libraryUid, bookUid, DateTime.Now);
        
        Assert.Equal(reservationUid, res.Reservation.ReservationUid);
        Assert.Equal(bookUid, res.Book.BookUid);
        Assert.Equal(libraryUid, res.Library.LibraryUid);
        _reservationHttpClientMock.Verify(l => l.GetReservationCountAsync(username), Times.Once);
        _ratingHttpClientMock.Verify(r => r.GetUserRatingAsync(username), Times.Once);
        _reservationHttpClientMock.Verify(r => 
            r.TakeBookAsync(username, libraryUid, bookUid, It.IsAny<DateTime>()), Times.Once);
        _libraryHttpClientMock.Verify(l => l.ChangeAvailableCountAsync(libraryUid, bookUid, -1), Times.Once);
        _libraryHttpClientMock.Verify(l => 
            l.GetLibrariesAndBooksAsync(new List<Guid> { libraryUid }, new List<Guid> { bookUid }), Times.Once);
    }

    [Theory, AutoData]
    public async Task ReturnBookTest(string username, Guid reservationUid, Guid libraryUid, Guid bookUid)
    {
        _reservationHttpClientMock.Setup(r => 
                r.ReturnBookAsync(reservationUid, It.IsAny<DateTime>()))
            .ReturnsAsync(new Reservation(reservationUid, Status.RETURNED, DateTime.Now, DateTime.Now, bookUid, libraryUid));
        _libraryHttpClientMock.Setup(l =>
            l.GetLibrariesAndBooksAsync(new List<Guid>(), new List<Guid> { bookUid }))
            .ReturnsAsync(([], [new Book(bookUid, "name", "author", "genre", Condition.EXCELLENT, 1)]));

        await _gatewayManager.ReturnBookAsync(username, reservationUid, Condition.BAD, DateTime.Now);
        
        _reservationHttpClientMock.Verify(l => l.ReturnBookAsync(reservationUid, It.IsAny<DateTime>()), Times.Once);
        _libraryHttpClientMock.Verify(l => l.ChangeAvailableCountAsync(libraryUid, bookUid, 1), Times.Once);
        _libraryHttpClientMock.Verify(l => 
            l.GetLibrariesAndBooksAsync(new List<Guid>(), new List<Guid> { bookUid }), Times.Once);
        _ratingHttpClientMock.Verify(r => r.UpdateUserRatingAsync(username, -10), Times.Once);
    }

    [Theory, AutoData]
    public async Task GetUserRatingTest(string username,[Range(0, 100)] int rating)
    {
        _ratingHttpClientMock.Setup(r => r.GetUserRatingAsync(username)).ReturnsAsync(rating);
        
        var res = await _gatewayManager.GetUserRatingAsync(username);
        
        Assert.Equal(rating, res);
        _ratingHttpClientMock.Verify(l => l.GetUserRatingAsync(username), Times.Once);
    }
}
