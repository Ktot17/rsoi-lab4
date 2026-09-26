using ReservationBL.Enums;
using ReservationBL.Models;

namespace ReservationBL.OutputPorts;

public interface IReservationManager
{
    public Task<IEnumerable<Reservation>> GetReservations(string username);
    public Task<Reservation> TakeBook(string username, Guid libraryUid, Guid bookUid, DateTime tillDate);
    public Task<Reservation> ReturnBook(Guid reservationUid, DateTime returnDate);
    public Task<int> GetRentedReservationCountAsync(string username);
}