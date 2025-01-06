using Microsoft.EntityFrameworkCore;
using N_Tier.Shared.N_Tier.Core.Common;
using System.Linq.Dynamic.Core;
using Auth.Application.Models.Account;
using Auth.Core.Entities;
using Auth.Core.Enums;
using N_Tier.Shared.N_Tier.DataAccess.Models;
using N_Tier.Shared.Services;
using Plainquire.Filter;

namespace N_Tier.DataAccess.Repositories.Impl;

public class BaseCompanyFilterRepository(IBaseRedisRepository baseRedisRepository, IClaimService claimService)
    : IBaseCompanyFilterRepository
{
    private async Task<IQueryable<TEntity>> CompanyFilterAsync<TEntity>(IQueryable<TEntity> queryable) where TEntity : InCompanyRefIdList
    {
        var userId = claimService.GetUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            var userRoles = await baseRedisRepository.GetAsync<List<UserRole>>(
                $"ForcegetUser_UserRole_{userId}");
            if (userRoles != null && userRoles.All(a => a.companyroletype != ECompanyRoleType.All))
            {
                var companyList = await baseRedisRepository.GetAsync<List<MyCompanyResponseDto>>(
                    $"ForcegetUser_MyCompanyResponse_{userId}");
                var companyRefList = companyList.Select(s => s.refid).ToList();

                queryable = queryable.Where(w =>
                    companyRefList.Contains(w.companyrefid.Value));
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
    
    public async Task<double> SumAsync<TEntity>(IQueryable<InCompanyRefIdSumList> queryable, EntityFilter<TEntity> where)
        where TEntity : InCompanyRefIdList
    {
        queryable = await CompanyFilterAsync(queryable);
        return await queryable.Where(where).SumAsync(f=>f.Value);
    }

    public async Task<List<TEntity>> GetAllGenericAsync<TEntity>(IQueryable<TEntity> queryable,
        GetAllRequest<TEntity> model) where TEntity : InCompanyRefIdList
    {
        queryable = queryable.Where(model.Filter);
        if (string.IsNullOrEmpty(model.OrderBy))
        {
            model.OrderBy = typeof(TEntity).GetProperty("createdon") != null ? "createdon DESC" : "id DESC";
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
