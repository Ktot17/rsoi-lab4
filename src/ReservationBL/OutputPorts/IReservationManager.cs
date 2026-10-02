using ReservationBL.Models;

namespace ReservationBL.OutputPorts;

public interface IReservationManager
{
    public Task<IEnumerable<Reservation>> GetReservationsAsync(string username);
    public Task<Reservation> TakeBookAsync(string username, Guid libraryUid, Guid bookUid, DateTime tillDate);
    public Task<Reservation> ReturnBookAsync(Guid reservationUid, DateTime returnDate);
    public Task<int> GetRentedReservationCountAsync(string username);
    public Task RevertTakeBookAsync(Guid reservationUid);
}
