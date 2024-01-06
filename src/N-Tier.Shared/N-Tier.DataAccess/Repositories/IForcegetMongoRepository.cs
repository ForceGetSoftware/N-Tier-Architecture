#nullable enable
using System.Linq.Expressions;
using MongoDB.Driver;
using N_Tier.Core.Entities;

namespace N_Tier.DataAccess.Repositories;

public interface IForcegetMongoRepository<T>
{
    IFindFluent<History<T>, History<T>> AsQuery(Expression<Func<History<T>, bool>> filter);
    Task<List<History<T>>> GetAllAsync();
    Task<List<History<T>>> GetAllAsync(Expression<Func<History<T>, bool>> filter);
    Task<History<T?>> GetAsync(string primaryRefId);
    Task CreateAsync(History<T> item);
    Task UpdateAsync(string primaryRefId, History<T> item);
    Task RemoveAsync(string primaryRefId);
}
