using Microsoft.EntityFrameworkCore;
using ReservationBL.Enums;
using ReservationBL.Exceptions;
using ReservationBL.InputPorts;
using ReservationBL.Models;
using ReservationDB.Models;

namespace ReservationDB.Postgres;

public class ReservationRepository(PostgresDbContext context) : IReservationRepository
{
    public async Task<IEnumerable<Reservation>> GetReservationsAsync(string username) => 
        await context.Reservations.Where(r => r.Username == username).Select(r => r.ToBlModel()).ToListAsync();

    public async Task<Reservation> CreateReservationAsync(string username, Guid libraryUid, Guid bookUid,
        DateTime tillDate)
    {
        var reservation = new ReservationDb(0, Guid.NewGuid(), username, bookUid, libraryUid,
            Status.Rented, DateTime.Now, tillDate);
        await context.Reservations.AddAsync(reservation);
        await context.SaveChangesAsync();
        return reservation.ToBlModel();
    }

    public async Task<Reservation> UpdateReservationStatusAsync(Guid reservationId, DateTime returnDate)
    {
        var reservation = await context.Reservations.FirstOrDefaultAsync(r => r.ReservationUid == reservationId)
                          ?? throw new EntityNotFoundException(typeof(ReservationDb));
        reservation.Status = returnDate > reservation.TillDate ? Status.Expired : Status.Returned;
        context.Reservations.Update(reservation);
        await context.SaveChangesAsync();
        return reservation.ToBlModel();
    }

    public async Task<int> GetRentedReservationCountAsync(string username) => 
        await context.Reservations.Where(r => r.Username == username).CountAsync();

    public async Task DeleteReservationAsync(Guid reservationId)
    {
        var reservation = await context.Reservations.FirstOrDefaultAsync(r => r.ReservationUid == reservationId) 
                          ?? throw new EntityNotFoundException(typeof(ReservationDb));
        context.Reservations.Remove(reservation);
        await context.SaveChangesAsync();
    }
}
