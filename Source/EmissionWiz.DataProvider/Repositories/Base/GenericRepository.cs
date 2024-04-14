using Autofac.Features.OwnedInstances;
using Autofac;
using EmissionWiz.Models.Database;

namespace EmissionWiz.DataProvider.Repositories.Base;

internal abstract class GenericRepository<TEntity> : BaseRepository<TEntity>
    where TEntity : class, new()
{
    protected IEnumerable<TEntity> LocalSet => Context.Set<TEntity>().Local;

    /// <summary>
    /// Will Find entity by id in context first, then in db.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public virtual Task<TEntity?> GetByIdAsync(Guid id)
    {
        return DbSet.FindAsync(id).AsTask();
    }

    public virtual void Delete(Guid id)
    {
        TEntity? entityToDelete = DbSet.Find(id);
        if (entityToDelete != null)
        {
            Delete(entityToDelete);
        }
    }

    public virtual async Task AddAndCommitAsync(TEntity item, Action<IDatabaseContext>? init = null)
    {
        await using var ctx = CreateScopedContext();

        ctx.Set<TEntity>().Add(item);
        init?.Invoke(ctx);
        await ctx.SaveChangesAsync();
    }

    protected IDatabaseContext CreateScopedContext()
    {
        var context = ComponentContext.Resolve<Owned<IDatabaseContext>>();
        return context.Value;
    }
}
