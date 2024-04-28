using Autofac;
using Nito.AsyncEx;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EmissionWiz.Models.Database;

public interface IDatabaseContext: IAsyncDisposable
{
    IModel Model { get; }
    DbSet<Substance> Substances { get; }
    DbSet<TempFile> TempFiles { get; }
    Task<int> CommitChangesAsync(CancellationToken cancellationToken = default);
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    DatabaseFacade Database { get; }
    Task BulkInsertAsync<T>(IList<T> entities);
    Task ResetChangesAsync();
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    public AsyncLock AsyncLock { get; }
}
public partial class EmissionWizDbContext : DbContext
{
    public virtual DbSet<Substance> Substances { get; set; } = null!;
    public virtual DbSet<TempFile> TempFiles { get; set; } = null!;

    public IComponentContext ComponentContext { get; }
    public AsyncLock AsyncLock { get; } = new AsyncLock();

    public EmissionWizDbContext(IComponentContext componentContext, DbContextOptions<EmissionWizDbContext> options) : base(options)
    {
        ComponentContext = componentContext;
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Substance>(entity =>
        {
            entity.Property(e => e.Code).ValueGeneratedNever();
        });
        modelBuilder.Entity<TempFile>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ContentType).IsUnicode(false);
            entity.Property(e => e.Label).IsUnicode(false);
        });
        
                    
    }
    //https://stackoverflow.com/questions/59624695/entity-framework-core-3-1-return-value-int-from-stored-procedure
    
}