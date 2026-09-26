using ReservationBL.InputPorts;
using ReservationBL.Models;
using ReservationBL.OutputPorts;

namespace ReservationBL.Managers;

public class ReservationManager(IReservationRepository reservationRepository) : IReservationManager
{
    public async Task<IEnumerable<Reservation>> GetReservations(string username) => 
        await reservationRepository.GetReservationsAsync(username);
    
    public async Task<Reservation> TakeBook(string username, Guid libraryUid, Guid bookUid, DateTime tillDate) => 
        await reservationRepository.CreateReservationAsync(username, libraryUid, bookUid, tillDate);

    public async Task<Reservation> ReturnBook(Guid reservationUid, DateTime returnDate) => 
        await reservationRepository.UpdateReservationStatusAsync(reservationUid, returnDate);
    
    public async Task<int> GetRentedReservationCountAsync(string username) => 
        await reservationRepository.GetRentedReservationCountAsync(username);
}