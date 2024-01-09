using System.Linq.Expressions;
using N_Tier.Core.Common;
using N_Tier.Core.Entities;

namespace N_Tier.DataAccess.Repositories;

public interface ICustomBaseRepository<TEntity> where TEntity : CustomBaseEntity
{
    IQueryable<TEntity> AsQueryable();

    Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

    Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> predicate);

    Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);

    Task<TEntity> AddAsync(TEntity entity);

    Task<TEntity> UpdateAsync(TEntity entity);

    Task<TEntity> DeleteAsync(TEntity entity);
    
    Task<List<History<TEntity>>> GetAllHistoryAsync(
        Expression<Func<History<TEntity>, bool>> filter = null);
    
    Task<TEntity> GetByRefIdAsync(Guid refId);

    Task<History<TEntity>> CreateHistoryAsync(History<TEntity> entity);
}