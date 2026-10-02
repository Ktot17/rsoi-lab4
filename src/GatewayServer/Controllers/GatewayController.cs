using System.Globalization;
using GatewayBL.Enums;
using GatewayBL.Exceptions;
using GatewayBL.Models;
using GatewayBL.OutputPorts;
using GatewayServer.Models;
using Microsoft.AspNetCore.Mvc;
using static System.Enum;

namespace GatewayServer.Controllers;

[ApiController]
[Route("api/v1/")]
public class GatewayController(IGatewayManager gatewayManager) : ControllerBase
{
    private const int DefaultPage = 1;
    private const int DefaultSize = 10;
    private const bool DefaultShowAll = false;
    private const string ServiceUnavailableMessageTitle = "Service Unavailable";
    private const string ServiceUnavailableMessage = "Bonus Service unavailable";

    [HttpGet("libraries")]
    public async Task<ActionResult<LibraryPaginationResponse>> GetLibraries(string city, int page = DefaultPage,
        int size = DefaultSize)
    {
        try
        {
            var (total, libraries) = await gatewayManager.GetLibrariesAsync(city, page, size);
            
            return Ok(new LibraryPaginationResponse(page, size, total, libraries.Select(l => 
                new LibraryResponse(l.LibraryUid, l.Name, l.Address, l.City))));
        }
        catch (ServiceIsUnavailableException e)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new
                {
                    status = StatusCodes.Status503ServiceUnavailable,
                    title = ServiceUnavailableMessageTitle,
                    detail = e.Message,
                    message = ServiceUnavailableMessage
                });
        }
    }

    [HttpGet("libraries/{libraryUid:guid}/books")]
    public async Task<ActionResult<LibraryBookPaginationResponse>> GetBooks(Guid libraryUid, int page = DefaultPage,
        int size = DefaultSize, bool showAll = DefaultShowAll)
    {
        try
        {
            var (total, books) = await gatewayManager.GetBooksAsync(libraryUid, page, size, showAll);

            return Ok(new LibraryBookPaginationResponse(page, size, total, books.Select(b =>
                new LibraryBookResponse(b.BookUid, b.Name, b.Author, b.Genre, b.Condition.ToString(),
                    b.AvailableCount))));
        }
        catch (ServiceIsUnavailableException e)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new
                {
                    status = StatusCodes.Status503ServiceUnavailable,
                    title = ServiceUnavailableMessageTitle,
                    detail = e.Message,
                    message = ServiceUnavailableMessage
                });
        }
    }

    [HttpGet("reservations")]
    public async Task<ActionResult<IEnumerable<BookReservationResponse>>> GetReservations(
        [FromHeader(Name = "X-User-Name")] string username)
    {
        try
        {
            var reservations = (await gatewayManager.GetReservationsAsync(username)).ToList();

            if (reservations.Exists(r => r.Book is null || r.Library is null))
                return StatusCode(StatusCodes.Status206PartialContent,
                    reservations.Select(r => new BookReservationWithUidsResponse(r.Reservation.ReservationUid,
                    r.Reservation.Status.ToString(),
                    DateOnly.FromDateTime(r.Reservation.StartDate).ToString("O", CultureInfo.InvariantCulture),
                    DateOnly.FromDateTime(r.Reservation.TillDate).ToString("O", CultureInfo.InvariantCulture),
                    r.Reservation.BookUid, r.Reservation.LibraryUid)));

            return Ok(reservations.Select(r => new BookReservationResponse(r.Reservation.ReservationUid,
                r.Reservation.Status.ToString(),
                DateOnly.FromDateTime(r.Reservation.StartDate).ToString("O", CultureInfo.InvariantCulture),
                DateOnly.FromDateTime(r.Reservation.TillDate).ToString("O", CultureInfo.InvariantCulture),
                new BookInfo(r.Book!.BookUid, r.Book.Name, r.Book.Author, r.Book.Genre),
                new LibraryResponse(r.Library!.LibraryUid, r.Library.Name, r.Library.Address, r.Library.City))));
        }
        catch (ServiceIsUnavailableException e)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new
                {
                    status = StatusCodes.Status503ServiceUnavailable,
                    title = ServiceUnavailableMessageTitle,
                    detail = e.Message,
                    message = ServiceUnavailableMessage
                });
        }
    }

    [HttpPost("reservations")]
    public async Task<ActionResult<TakeBookResponse>> TakeBook([FromHeader(Name = "X-User-Name")] string username,
        [FromBody] TakeBookRequest request)
    {
        FullInfo reservation;

        try
        {
            reservation =
                await gatewayManager.TakeBookAsync(username, request.LibraryUid, request.BookUid, request.TillDate);
        }
        catch (TooManyRentedBooksException)
        {
            return BadRequest(new ValidationErrorResponse("", []));
        }
        catch (ServiceIsUnavailableException e)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new
                {
                    status = StatusCodes.Status503ServiceUnavailable,
                    title = ServiceUnavailableMessageTitle,
                    detail = e.Message,
                    message = ServiceUnavailableMessage
                });
        }

        if (reservation.Book is null || reservation.Library is null)
            return StatusCode(StatusCodes.Status206PartialContent, new TakeBookWithUidsResponse(
                reservation.Reservation.ReservationUid,
                reservation.Reservation.Status.ToString(),
                DateOnly.FromDateTime(reservation.Reservation.StartDate)
                    .ToString("O", CultureInfo.InvariantCulture),
                DateOnly.FromDateTime(reservation.Reservation.TillDate)
                    .ToString("O", CultureInfo.InvariantCulture),
                reservation.Reservation.BookUid, reservation.Reservation.LibraryUid,
                new UserRatingResponse(reservation.Rating)));
        
        return Ok(new TakeBookResponse(reservation.Reservation.ReservationUid,
            reservation.Reservation.Status.ToString(),
            DateOnly.FromDateTime(reservation.Reservation.StartDate)
                .ToString("O", CultureInfo.InvariantCulture),
            DateOnly.FromDateTime(reservation.Reservation.TillDate)
                .ToString("O", CultureInfo.InvariantCulture),
            new BookInfo(reservation.Book.BookUid, reservation.Book.Name, reservation.Book.Author,
                reservation.Book.Genre),
            new LibraryResponse(reservation.Library.LibraryUid, reservation.Library.Name, reservation.Library.Address,
                reservation.Library.City), new UserRatingResponse(reservation.Rating)));
    }

    [HttpPost("reservations/{reservationUid:guid}/return")]
    public async Task<ActionResult> ReturnBook([FromHeader(Name = "X-User-Name")] string username, Guid reservationUid,
        [FromBody] ReturnBookRequest request)
    {
        if (!TryParse(request.Condition, out Condition condition))
            return BadRequest(new ValidationErrorResponse("", [new ErrorDescription(nameof(request.Condition), "")]));
        try
        {
            await gatewayManager.ReturnBookAsync(username, reservationUid, condition, request.Date);
        }
        catch (EntityNotFoundException)
        {
            return NotFound(new ErrorResponse("No reservation found"));
        }
        catch (ServiceIsUnavailableException e)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new
                {
                    status = StatusCodes.Status503ServiceUnavailable,
                    title = ServiceUnavailableMessageTitle,
                    detail = e.Message,
                    message = ServiceUnavailableMessage
                });
        }
        
        return NoContent();
    }

    [HttpGet("rating")]
    public async Task<ActionResult<UserRatingResponse>> GetUserRating(
        [FromHeader(Name = "X-User-Name")] string username)
    {
        try
        {
            var rating = await gatewayManager.GetUserRatingAsync(username);
            return Ok(new UserRatingResponse(rating));
        }
        catch (ServiceIsUnavailableException e)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new
                {
                    status = StatusCodes.Status503ServiceUnavailable,
                    title = ServiceUnavailableMessageTitle,
                    detail = e.Message,
                    message = ServiceUnavailableMessage
                });
        }
    }
}
