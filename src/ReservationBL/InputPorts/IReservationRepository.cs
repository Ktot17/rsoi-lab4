using ReservationBL.Models;

namespace ReservationBL.InputPorts;

public interface IReservationRepository
{
    public Task<IEnumerable<Reservation>> GetReservationsAsync(string username);
    public Task<Reservation> CreateReservationAsync(string username, Guid libraryUid, Guid bookUid, DateTime tillDate);
    public Task<Reservation> UpdateReservationStatusAsync(Guid reservationId, DateTime returnDate);
    public Task<int> GetRentedReservationCountAsync(string username);
    public Task DeleteReservationAsync(Guid reservationId);
}
