using System.Linq.Expressions;
using MongoDB.Driver;
using N_Tier.Core.Entities;

namespace N_Tier.DataAccess.Repositories;

public interface IForcegetMongoFuncRepository
{
    IFindFluent<History<T>, History<T>> AsQuery<T>(Expression<Func<History<T>, bool>> filter);
    Task<List<History<T>>> GetAllAsync<T>();
    Task<List<History<T>>> GetAllAsync<T>(Expression<Func<History<T>, bool>> filter);
    Task<History<T>?> GetAsync<T>(string primaryRefId);
    Task<History<T>?> GetLatestAsync<T>(string primaryRefId);
    Task CreateAsync<T>(History<T> item);
    Task UpdateAsync<T>(string primaryRefId, History<T> item);
    Task UpdateAllAsync<T>(IEnumerable<History<T>> documents, Func<History<T>, FilterDefinition<History<T>>> filterExpression);
    Task RemoveAsync<T>(string primaryRefId);
    Task RemoveAllAsync<T>(Expression<Func<History<T>, bool>> filter);
    Task SaveAsync<T>(string primaryRefId, T element);
}
