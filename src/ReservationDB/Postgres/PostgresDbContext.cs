using Microsoft.EntityFrameworkCore;
using ReservationDB.Models;

namespace ReservationDB.Postgres;

public class PostgresDbContext : DbContext
{
    private const string TimeStamp = "timestamp";
    
    public PostgresDbContext() {}

    public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options) {}

    public virtual DbSet<ReservationDb> Reservations { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<ReservationDb>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.ReservationUid).IsUnique();
            entity.Property(e => e.ReservationUid).IsRequired();
            entity.Property(e => e.Username).IsRequired();
            entity.Property(e => e.BookUid).IsRequired();
            entity.Property(e => e.LibraryUid).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.StartDate).IsRequired().HasColumnType(TimeStamp);
            entity.Property(e => e.TillDate).IsRequired().HasColumnType(TimeStamp);
        });
    }
}