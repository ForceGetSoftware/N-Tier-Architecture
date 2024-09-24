using FS.FilterExpressionCreator.Filters;
using N_Tier.Shared.N_Tier.Core.Common;
using System.Linq.Expressions;

namespace N_Tier.DataAccess.Repositories;

public interface IBaseGenericRepository
{
    IQueryable<TEntity> AsQueryable<TEntity>() where TEntity : class;

    Task<TEntity> GetFirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;

    Task<TEntity> GetFirstAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;

    Task<List<TEntity>> GetAllAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;

    Task<TEntity> AddAsync<TEntity>(TEntity entity) where TEntity : class;

    Task<int> AddRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

    Task<TEntity> UpdateAsync<TEntity>(TEntity entity) where TEntity : class;

    Task<int> UpdateRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

    Task<TEntity> DeleteAsync<TEntity>(TEntity entity) where TEntity : class;

    Task<TEntity> DeleteAsync<TEntity>(TEntity entity, bool hardDelete) where TEntity : class;

    Task<int> DeleteRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

    Task<int> DeleteRangeAsync<TEntity>(IEnumerable<TEntity> entities, bool hardDelete) where TEntity : class;

    Task<int> CountAsync<TEntity>(IQueryable<TEntity> queryable, EntityFilter<TEntity> where) where TEntity : class;

    Task<int> CountAsync<TEntity>(GetAllRequest<TEntity> model) where TEntity : class;

    Task<List<TEntity>> GetAllGenericAsync<TEntity>(GetAllRequest<TEntity> model) where TEntity : class;

    Task<List<TEntity>> GetAllGenericAsync<TEntity>(IQueryable<TEntity> queryable, GetAllRequest<TEntity> model) where TEntity : class;
}
