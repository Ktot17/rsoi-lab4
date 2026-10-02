using Microsoft.EntityFrameworkCore;
using RatingDB.Models;

namespace RatingDB.Postgres;

public class PostgresDbContext : DbContext
{
    public PostgresDbContext() {}

    public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options) {}

    public virtual DbSet<RatingDb> Ratings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<RatingDb>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Username).IsRequired();
            entity.Property(e => e.Stars).IsRequired();
            entity.ToTable(t => t.HasCheckConstraint("rating_stars", "\"stars\" >= 0 AND \"stars\" <= 100"));
        });
    }
}