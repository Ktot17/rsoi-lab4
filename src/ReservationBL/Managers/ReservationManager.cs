using ReservationBL.InputPorts;
using ReservationBL.Models;
using ReservationBL.OutputPorts;

namespace ReservationBL.Managers;

public class ReservationManager(IReservationRepository reservationRepository) : IReservationManager
{
    public async Task<IEnumerable<Reservation>> GetReservationsAsync(string username) => 
        await reservationRepository.GetReservationsAsync(username);
    
    public async Task<Reservation> TakeBookAsync(string username, Guid libraryUid, Guid bookUid, DateTime tillDate) => 
        await reservationRepository.CreateReservationAsync(username, libraryUid, bookUid, tillDate);

    public async Task<Reservation> ReturnBookAsync(Guid reservationUid, DateTime returnDate) => 
        await reservationRepository.UpdateReservationStatusAsync(reservationUid, returnDate);
    
    public async Task<int> GetRentedReservationCountAsync(string username) => 
        await reservationRepository.GetRentedReservationCountAsync(username);
    
    public async Task RevertTakeBookAsync(Guid reservationUid) =>
        await reservationRepository.DeleteReservationAsync(reservationUid);
}
