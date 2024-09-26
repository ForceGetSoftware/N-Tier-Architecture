using N_Tier.Application.Models;
using N_Tier.DataAccess.Repositories;
using N_Tier.Shared.N_Tier.Core.Common;
using System.Linq.Expressions;

namespace N_Tier.Application.Services.Impl;

public class BaseService<TEntity>(IBaseRepository<TEntity> repository) : IBaseService<TEntity>
{
    private readonly IBaseRepository<TEntity> repository = repository;

    public Task<TEntity> AddAsync(TEntity entity)
    {
        return repository.AddAsync(entity);
    }

    public Task<TEntity> UpdateAsync(TEntity entity)
    {
        return repository.UpdateAsync(entity);
    }

    public Task<TEntity> DeleteAsync(TEntity entity)
    {
        return repository.DeleteAsync(entity);
    }

    public async Task<ApiListResult<List<TEntity>>> GetAllGenericAsync(GetAllRequest<TEntity> model)
    {
        return ApiListResult<List<TEntity>>.Success(await repository.GetAllGenericAsync(model),
            await repository.CountAsync(model));
    }

    public Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return repository.GetFirstOrDefaultAsync(predicate);
    }

    public Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return repository.GetFirstAsync(predicate);
    }
}
