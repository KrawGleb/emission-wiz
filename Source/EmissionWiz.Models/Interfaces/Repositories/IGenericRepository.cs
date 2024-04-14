using EmissionWiz.Models.Database;

namespace EmissionWiz.Models.Interfaces.Repositories;

public interface IGenericRepository<TEntity, in TKey> : IBaseRepository<TEntity>
    where TEntity : class, new()
    where TKey : struct
{
    Task<TEntity?> GetByIdAsync(TKey id);
}

public interface IGenericRepository<TEntity> : IGenericRepository<TEntity, Guid>
    where TEntity : class, new()
{
    Task AddAndCommitAsync(TEntity item, Action<IDatabaseContext>? init = null);
}