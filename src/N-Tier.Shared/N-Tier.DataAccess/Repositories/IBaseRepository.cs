using FS.FilterExpressionCreator.Filters;
using N_Tier.Core.Entities;
using N_Tier.Shared.N_Tier.Application.Models;
using N_Tier.Shared.N_Tier.Core.Common;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace N_Tier.DataAccess.Repositories;

public interface IBaseRepository<TEntity>
{
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task RollbackTransactionAsync();
    Task CommitTransactionAsync();
    IQueryable<TEntity> AsQueryable();

    Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

    Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> predicate);

    Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);

    Task<List<History<dynamic>>> GetAllHistory(HistoryRequest model);

    Task<TEntity> AddAsync(TEntity entity);

    Task<int> AddRangeAsync(List<TEntity> entities);

    Task<TEntity> UpdateAsync(TEntity entity);

    Task<int> UpdateRangeAsync(List<TEntity> entities);

    Task<TEntity> DeleteAsync(TEntity entity);

    Task<TEntity> DeleteAsync(TEntity entity, bool hardDelete);

    Task<int> DeleteRangeAsync(List<TEntity> entities);

    Task<int> DeleteRangeAsync(List<TEntity> entities, bool hardDelete);

    Task<int> CountAsync<TEntity>(IQueryable<TEntity> queryable, EntityFilter<TEntity> where);

    Task<int> CountAsync(GetAllRequest<TEntity> model);

    Task<List<TEntity>> GetAllGenericAsync(GetAllRequest<TEntity> model);

    Task<List<TEntity>> GetAllGenericAsync<TEntity>(IQueryable<TEntity> queryable, GetAllRequest<TEntity> model);
}
