using System.ComponentModel.DataAnnotations.Schema;
using ReservationBL.Enums;
using ReservationBL.Models;

namespace ReservationDB.Models;

[Table("reservation")]
public class ReservationDb(int id, Guid reservationUid, string username, Guid bookUid, Guid libraryUid,
    Status status, DateTime startDate, DateTime tillDate)
{
    public ReservationDb(Reservation reservation) : this(0, reservation.ReservationUid, reservation.Username,
        reservation.BookUid, reservation.LibraryUid, reservation.Status, reservation.StartDate, reservation.TillDate) {}

    [Column("id")]
    public int Id { get; init; } = id;
    
    [Column("reservation_uid")]
    public Guid ReservationUid { get; init; } = reservationUid;
    
    [Column("username")]
    public string Username { get; init; } = username;
    
    [Column("book_uid")]
    public Guid BookUid { get; init; } = bookUid;
    
    [Column("library_uid")]
    public Guid LibraryUid { get; init; } = libraryUid;
    
    [Column("status")]
    public Status Status { get; set; } = status;
    
    [Column("start_date")]
    public DateTime StartDate { get; init; } = startDate;
    
    [Column("till_date")]
    public DateTime TillDate { get; init; } = tillDate;
    
    public Reservation ToBlModel() => new Reservation(ReservationUid, Username, BookUid, LibraryUid, Status, StartDate, TillDate);
}