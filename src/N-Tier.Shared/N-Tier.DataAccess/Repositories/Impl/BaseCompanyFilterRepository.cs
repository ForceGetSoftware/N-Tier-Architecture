using Microsoft.EntityFrameworkCore;
using N_Tier.Shared.N_Tier.Core.Common;
using System.Linq.Dynamic.Core;
using Auth.Application.Models.Account;
using Auth.Core.Entities;
using N_Tier.Shared.N_Tier.DataAccess.Models;
using N_Tier.Shared.Services;
using Plainquire.Filter;

namespace N_Tier.DataAccess.Repositories.Impl;

public class BaseCompanyFilterRepository : IBaseCompanyFilterRepository
{
    private readonly IBaseRedisRepository _baseRedisRepository;
    private readonly IClaimService _claimService;

    public BaseCompanyFilterRepository(
        IBaseRedisRepository baseRedisRepository, IClaimService claimService)
    {
        _baseRedisRepository = baseRedisRepository;
        _claimService = claimService;
    }

    private async Task<IQueryable<TEntity>> CompanyFilterAsync<TEntity>(IQueryable<TEntity> queryable)
    {
        var userId = _claimService.GetUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            var roles = await _baseRedisRepository.GetAsync<List<ForcegetRole>>(
                $"ForcegetUser_ForcegetRole_{userId}");
            if (roles != null && roles.All(a => a.name != "Admin"))
            {
                var companyList = await _baseRedisRepository.GetAsync<List<MyCompanyResponseDto>>(
                    $"ForcegetUser_MyCompanyResponse_{userId}");
                var companyRefList = companyList.Select(s => s.refid).ToList();

                queryable = queryable.Where(w =>
                    companyRefList.Contains((w as InCompanyRefIdList).companyrefid.Value));
            }
        }
        else
        {
            throw new Exception("Bearer Token User id is null");
        }

        return queryable;
    }

    public async Task<int> CountAsync<TEntity>(IQueryable<TEntity> queryable, EntityFilter<TEntity> where)
        where TEntity : InCompanyRefIdList
    {
        queryable = await CompanyFilterAsync(queryable);
        return await queryable.CountAsync(where);
    }


    public async Task<List<TEntity>> GetAllGenericAsync<TEntity>(IQueryable<TEntity> queryable,
        GetAllRequest<TEntity> model) where TEntity : InCompanyRefIdList
    {
        queryable = queryable.Where(model.Filter);
        if (string.IsNullOrEmpty(model.OrderBy))
        {
            if (typeof(TEntity).GetProperty("createdon") != null)
                model.OrderBy = "createdon DESC";
            else if (typeof(TEntity).GetProperty("refid") != null)
                model.OrderBy = "refid DESC";
            else
                model.OrderBy = "id DESC";
        }

        if (!string.IsNullOrEmpty(model.OrderBy))
            queryable = queryable.OrderBy(model.OrderBy);

        queryable = await CompanyFilterAsync(queryable);

        return await queryable
            .Skip(model.Skip)
            .Take(model.Take)
            .ToListAsync();
    }
}
