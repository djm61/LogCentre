using System.Linq.Expressions;

namespace LogCentre.Services.Interfaces
{
    public interface IService<TKey, TEntity> where TKey : struct
    {
        bool TryGet(TKey id, out TEntity entity);
        Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            int page = 0,
            int pageSize = 0);
        Task<TEntity> GetAsync(TKey id);
        Task<TEntity> CreateAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task RemoveAsync(TEntity entity);
        bool Exists(TEntity entity);
        bool IsTracked(TEntity entity);
    }
}
