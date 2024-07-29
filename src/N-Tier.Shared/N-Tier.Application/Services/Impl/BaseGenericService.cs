using System.Linq.Expressions;
using N_Tier.Application.Models;
using N_Tier.DataAccess.Repositories;
using N_Tier.Shared.N_Tier.Core.Common;

namespace N_Tier.Application.Services.Impl;

public class BaseGenericService<TEntity>(IBaseGenericRepository repository) : IBaseService<TEntity> where TEntity : class
{
    public Task<TEntity> AddAsync(TEntity entity)
    {
        return repository.AddAsync<TEntity>(entity);
    }
    
    public Task<TEntity> UpdateAsync(TEntity entity)
    {
        return repository.UpdateAsync<TEntity>(entity);
    }
    
    public Task<TEntity> DeleteAsync(TEntity entity)
    {
        return repository.DeleteAsync<TEntity>(entity);
    }
    
    public async Task<ApiListResult<List<TEntity>>> GetAllGenericAsync(GetAllRequest<TEntity> model)
    {
        return ApiListResult<List<TEntity>>.Success(await repository.GetAllGenericAsync<TEntity>(model),
            await repository.CountAsync<TEntity>(model));
    }
    
    public Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return repository.GetFirstOrDefaultAsync<TEntity>(predicate);
    }
    
    public Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return repository.GetFirstAsync<TEntity>(predicate);
    }
}
