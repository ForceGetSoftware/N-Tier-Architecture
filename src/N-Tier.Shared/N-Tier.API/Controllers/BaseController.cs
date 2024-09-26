using FS.FilterExpressionCreator.Filters;
using Microsoft.AspNetCore.Mvc;
using N_Tier.Application.Models;
using N_Tier.Application.Services;
using N_Tier.Shared.N_Tier.Core.Common;

namespace N_Tier.API.Controllers;

public class BaseController<TEntity>(IBaseService<TEntity> service) : ApiController
{
    [HttpGet("GetAllAsync")]
    public virtual async Task<IActionResult> GetAllAsync([FromQuery] EntityFilter<TEntity> filter, int take, int skip,
        string search, string orderBy)
    {
        return Ok(await service.GetAllGenericAsync(new GetAllRequest<TEntity>
        {
            Filter = filter,
            Take = take,
            Skip = skip,
            Search = search,
            OrderBy = orderBy
        }));
    }

    [HttpPost("AddAsync")]
    public virtual async Task<IActionResult> AddAsync(TEntity entity)
    {
        return Ok(ApiResult<TEntity>.Success(await service.AddAsync(entity)));
    }

    [HttpPut("UpdateAsync")]
    public virtual async Task<IActionResult> UpdateAsync(TEntity entity)
    {
        return Ok(ApiResult<TEntity>.Success(await service.UpdateAsync(entity)));
    }

    [HttpDelete("DeleteAsync")]
    public virtual async Task<IActionResult> DeleteAsync(TEntity entity)
    {
        return Ok(ApiResult<TEntity>.Success(await service.DeleteAsync(entity)));
    }
}
