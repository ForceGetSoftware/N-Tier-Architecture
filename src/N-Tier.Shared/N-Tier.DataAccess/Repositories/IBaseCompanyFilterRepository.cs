using N_Tier.Shared.N_Tier.Core.Common;
using N_Tier.Shared.N_Tier.DataAccess.Models;
using Plainquire.Filter;

namespace N_Tier.DataAccess.Repositories;

public interface IBaseCompanyFilterRepository
{
    Task<int> CountAsync<TEntity>(IQueryable<TEntity> queryable, EntityFilter<TEntity> where) where TEntity : InCompanyRefIdList;

    Task<List<TEntity>> GetAllGenericAsync<TEntity>(IQueryable<TEntity> queryable, GetAllRequest<TEntity> model) where TEntity : InCompanyRefIdList;

    Task<double> SumAsync<TEntity>(IQueryable<InCompanyRefIdSumList> queryable, EntityFilter<TEntity> where)
        where TEntity : InCompanyRefIdList;
}
