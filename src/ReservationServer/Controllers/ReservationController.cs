using Microsoft.AspNetCore.Mvc;
using ReservationBL.Exceptions;
using ReservationBL.Models;
using ReservationBL.OutputPorts;
using ReservationServer.Models;

namespace ReservationServer.Controllers;

[ApiController]
[Route("api/v1/reservations/")]
public class ReservationController(IReservationManager reservationManager) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReservationResponse>>> GetReservationsAsync([FromHeader(Name = "X-User-Name")] string username)
    {
        var reservations = await reservationManager.GetReservationsAsync(username);
        return Ok(reservations.Select(r => new ReservationResponse(r)));
    }

    [HttpPost]
    public async Task<ActionResult<ReservationResponse>> TakeBookAsync(
        [FromHeader(Name = "X-User-Name")] string username, [FromBody] TakeBookRequest request)
    {
        var reservation = await reservationManager.TakeBookAsync(username, request.LibraryUid, request.BookUid, request.TillDate);
        return Ok(new ReservationResponse(reservation));
    }

    [HttpPost("{reservationUid:guid}/return")]
    public async Task<ActionResult<ReservationResponse>> ReturnBookAsync(Guid reservationUid, [FromBody] ReturnBookRequest returnDate)
    {
        Reservation reservation;
        
        try
        {
            reservation = await reservationManager.ReturnBookAsync(reservationUid, returnDate.Date);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
        
        return Ok(new ReservationResponse(reservation));
    }

    [HttpGet("count")]
    public async Task<ActionResult<int>> GetReservationCountAsync([FromHeader(Name = "X-User-Name")] string username)
    {
        var count = await reservationManager.GetRentedReservationCountAsync(username);
        return Ok(count);
    }

    [HttpDelete("{reservationUid:guid}")]
    public async Task<ActionResult> DeleteReservationAsync(Guid reservationUid)
    {
        await reservationManager.RevertTakeBookAsync(reservationUid);
        return NoContent();
    }
}
