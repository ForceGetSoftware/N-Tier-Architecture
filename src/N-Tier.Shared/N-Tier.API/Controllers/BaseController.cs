using Microsoft.AspNetCore.Mvc;
using N_Tier.Application.Models;
using N_Tier.Application.Services;
using N_Tier.Shared.N_Tier.Core.Common;
using Plainquire.Filter;

namespace N_Tier.API.Controllers;

public class BaseController<TEntity>(IBaseService<TEntity> service) : ApiController
{
    [HttpGet("GetAllAsync")]
    public virtual async Task<ApiListResult<List<TEntity>>> GetAllAsync([FromQuery] EntityFilter<TEntity> filter,
        int take, int skip,
        string search, string orderBy) =>
        await service.GetAllGenericAsync(new GetAllRequest<TEntity>()
        {
            Filter = filter, Skip = skip, Take = take,
            Search = search, OrderBy = orderBy
        });

    [HttpPost("AddAsync")]
    public virtual async Task<ApiResult<TEntity>> AddAsync(TEntity entity) =>
        ApiResult<TEntity>.Success(await service.AddAsync(entity));

    [HttpPut("UpdateAsync")]
    public virtual async Task<ApiResult<TEntity>> UpdateAsync(TEntity entity) =>
        ApiResult<TEntity>.Success(await service.UpdateAsync(entity));

    [HttpDelete("DeleteAsync")]
    public virtual async Task<ApiResult<TEntity>> DeleteAsync(TEntity entity) =>
        ApiResult<TEntity>.Success(await service.DeleteAsync(entity));
}
