using Autofac;
using EmissionWiz.Models.Database;
using EmissionWiz.Models.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EmissionWiz.DataProvider.Repositories.Base;

internal class BaseRepository : IBaseRepository
{
    public IDatabaseContext Context { get; set; } = null!;
    public IComponentContext ComponentContext { get; set; } = null!;
    public IHttpContextAccessor HttpContextAccessor { get; set; } = null!;

    protected CancellationToken CancellationToken
    {
        get
        {
            if (HttpContextAccessor.HttpContext != null)
            {
                return HttpContextAccessor.HttpContext.RequestAborted;
            }

            return CancellationToken.None;
        }
    }
}

internal abstract class BaseRepository<TEntity> : BaseRepository, IBaseRepository<TEntity>
    where TEntity : class, new()
{
    public DbSet<TEntity> DbSet => Context.Set<TEntity>();

    public virtual void Add(TEntity entity)
    {
        DbSet.Add(entity);
    }

    public virtual void AddRange(IEnumerable<TEntity> entities)
    {
        DbSet.AddRange(entities);
    }

    public void Attach(TEntity item)
    {
        DbSet.Attach(item);
    }

    public virtual void Delete(TEntity entityToDelete)
    {
        DbSet.Remove(entityToDelete);
    }

    public virtual void Update(TEntity entityToUpdate)
    {
        var entry = Context.Entry(entityToUpdate);
        if (entry.State != EntityState.Added)
        {
            entry.State = EntityState.Modified;
        }
    }

    public virtual IQueryable<TEntity> Read(Expression<Func<TEntity, bool>>? predicate = null)
    {
        return predicate != null ? DbSet.Where(predicate) : DbSet;
    }
}

