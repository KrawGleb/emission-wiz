using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EmissionWiz.Models.Database;

public interface IDatabaseContext : IAsyncDisposable
{
    DbSet<CalculationResult> CalculationResults { get; set; }
    DbSet<Report> Reports { get; set; }

    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    Task<int> CommitChangesAsync(CancellationToken cancellationToken = default);
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    DatabaseFacade Database { get; }
}

public partial class EmissionWizDbContext : DbContext
{
    public EmissionWizDbContext(DbContextOptions<EmissionWizDbContext> options)
        : base(options)
    { }

    public virtual DbSet<CalculationResult> CalculationResults { get; set; } = null!;
    public virtual DbSet<Report> Reports { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CalculationResult>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
        });
        modelBuilder.Entity<Report>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Label).IsUnicode(false);
            entity.Property(e => e.ContentType).IsUnicode(false);
            entity.HasOne(e => e.CalculationResult)
                .WithMany(c => c.Reports)
                .HasForeignKey(e => e.OperationId)
                .HasConstraintName("FK_Report_CalculationResult");
        });
    }
}
