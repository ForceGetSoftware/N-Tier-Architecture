using System.Linq.Expressions;

namespace N_Tier.DataAccess.Repositories;

public interface IForcegetBaseRepository<TEntity>
{
    IQueryable<TEntity> AsQueryable();
    
    Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
    
    Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> predicate);
    
    Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);
    
    Task<TEntity> AddAsync(TEntity entity);
    
    Task<TEntity> UpdateAsync(TEntity entity);
    
    Task<TEntity> DeleteAsync(TEntity entity);
}
