using FS.FilterExpressionCreator.Filters;
using Microsoft.EntityFrameworkCore.Query;
using N_Tier.Application.Models;
using N_Tier.Shared.N_Tier.Core.Common;
using System.Linq.Expressions;

namespace N_Tier.DataAccess.Repositories;

public interface IBaseRepository<TEntity>
{
    IQueryable<TEntity> AsQueryable();

    Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

    Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> predicate);

    Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);

    Task<TEntity> AddAsync(TEntity entity);

    Task<int> AddRangeAsync(IEnumerable<TEntity> entities);

    Task<TEntity> UpdateAsync(TEntity entity);

    Task<int> UpdateRangeAsync(IEnumerable<TEntity> entities);

    Task<TEntity> DeleteAsync(TEntity entity);

    Task<TEntity> DeleteAsync(TEntity entity, bool hardDelete);

    Task<int> DeleteRangeAsync(IEnumerable<TEntity> entities);

    Task<int> DeleteRangeAsync(IEnumerable<TEntity> entities, bool hardDelete);

    Task<ApiListResult<List<TResult>>> GetFilteredListAsync<TResult>(
        Expression<Func<TEntity, TResult>> select, Expression<Func<TEntity, bool>> where,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
        int skip = 0, int take = 10);

    Task<int> CountAsync<TEntity>(IQueryable<TEntity> queryable, EntityFilter<TEntity> where);

    Task<int> CountAsync(GetAllRequest<TEntity> model);

    Task<List<TEntity>> GetAllGenericAsync(GetAllRequest<TEntity> model);

    Task<List<TEntity>> GetAllGenericAsync<TEntity>(IQueryable<TEntity> queryable, GetAllRequest<TEntity> model);
}
