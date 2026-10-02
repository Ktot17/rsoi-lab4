using LibraryBL.Enums;
using LibraryDB.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryDB.Postgres;

public class PostgresDbContext : DbContext
{
    public PostgresDbContext() {}
    
    public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options) {}

    public virtual DbSet<LibraryDb> Libraries { get; set; } = null!;
    public virtual DbSet<BookDb> Books { get; set; } = null!;
    public virtual DbSet<LibraryBookDb> LibraryBooks { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<LibraryDb>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.LibraryUid).IsUnique();
            entity.Property(e => e.LibraryUid).IsRequired();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.City).IsRequired();
            entity.Property(e => e.Address).IsRequired();
        });

        modelBuilder.Entity<BookDb>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.BookUid).IsUnique();
            entity.Property(e => e.BookUid).IsRequired();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Author);
            entity.Property(e => e.Genre);
            entity.Property(e => e.Condition).HasDefaultValue(Condition.Excellent);
        });

        modelBuilder.Entity<LibraryBookDb>(entity =>
        {
            entity.HasKey(e => new { e.LibraryId, e.BookId });
            entity.HasOne<LibraryDb>().WithMany().HasForeignKey(e => e.LibraryId);
            entity.HasOne<BookDb>().WithMany().HasForeignKey(e => e.BookId);
            entity.Property(e => e.AvailableCount).IsRequired();
        });
    }
}