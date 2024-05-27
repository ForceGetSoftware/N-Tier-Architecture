using System.Linq.Expressions;
using FS.FilterExpressionCreator.Filters;
using Microsoft.EntityFrameworkCore.Query;
using N_Tier.Application.Models;
using N_Tier.Shared.N_Tier.Core.Common;

namespace N_Tier.DataAccess.Repositories;

public interface IBaseRepository<TEntity>
{
    IQueryable<TEntity> AsQueryable();
    Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> predicate);
    
    Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);
    
    Task<TEntity> AddAsync(TEntity entity);
    
    Task<TEntity> UpdateAsync(TEntity entity);
    
    Task<TEntity> DeleteAsync(TEntity entity);
    
    Task<ApiListResult<List<TResult>>> GetFilteredListAsync<TResult>(
        Expression<Func<TEntity, TResult>> select, Expression<Func<TEntity, bool>> where,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
        int skip = 0, int take = 10);
    
    Task<int> CountAsync<TEntity>(IQueryable<TEntity> queryable, EntityFilter<TEntity> where);
    Task<List<TEntity>> GetAllGenericAsync(GetAllRequest<TEntity> model);
    Task<List<TEntity>> GetAllGenericAsync<TEntity>(IQueryable<TEntity> queryable, GetAllRequest<TEntity> model);
}
