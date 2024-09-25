using N_Tier.Application.Models;
using N_Tier.Core.Entities;
using N_Tier.DataAccess.Repositories;
using N_Tier.Shared.N_Tier.Application.Models;
using N_Tier.Shared.N_Tier.Core.Common;
using System.Linq.Expressions;

namespace N_Tier.Application.Services.Impl;

public class BaseService<TEntity> : IBaseService<TEntity>
{
    private readonly IBaseRepository<TEntity> repository;
    private readonly IForcegetMongoFuncRepository forcegetMongoFuncRepository;

    public BaseService(IBaseRepository<TEntity> repository)
    {
        this.repository = repository;
    }

    public BaseService(IBaseRepository<TEntity> repository, IForcegetMongoFuncRepository forcegetMongoFuncRepository)
    {
        this.repository = repository;
        this.forcegetMongoFuncRepository = forcegetMongoFuncRepository;
    }

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

    public async Task<ApiListResult<List<History<TEntity>>>> GetAllHistory(HistoryRequest model)
    {
        var list = await forcegetMongoFuncRepository.GetAllAsync<TEntity>(model);
        return ApiListResult<List<History<TEntity>>>.Success(list, list.Count);
    }
}
