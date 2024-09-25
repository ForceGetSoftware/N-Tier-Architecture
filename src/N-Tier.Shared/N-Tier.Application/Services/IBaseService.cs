using N_Tier.Application.Models;
using N_Tier.Core.Entities;
using N_Tier.Shared.N_Tier.Application.Models;
using N_Tier.Shared.N_Tier.Core.Common;

namespace N_Tier.Application.Services;

public interface IBaseService<TEntity>
{
    Task<TEntity> AddAsync(TEntity entity);

    Task<TEntity> UpdateAsync(TEntity entity);

    Task<TEntity> DeleteAsync(TEntity entity);

    Task<ApiListResult<List<TEntity>>> GetAllGenericAsync(GetAllRequest<TEntity> model);

    Task<ApiListResult<List<History<TEntity>>>> GetAllHistory(HistoryRequest model);
}
