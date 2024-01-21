using System.Linq.Expressions;

namespace EmissionWiz.Models.Interfaces.Repositories;

public interface IBaseRepository
{
}

public interface IBaseRepository<TEntity> : IBaseRepository
    where TEntity : class, new()
{
    void Add(TEntity item);
    void AddRange(IEnumerable<TEntity> entities);
    IQueryable<TEntity> Read(Expression<Func<TEntity, bool>>? predicate = null);
    void Update(TEntity item);
    void Attach(TEntity item);
    void Delete(TEntity item);
}