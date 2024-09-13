using System.Linq.Expressions;
using MongoDB.Driver;
using N_Tier.Application.Models;
using N_Tier.Core.Entities;

namespace N_Tier.DataAccess.Repositories.Impl;

public interface IBaseMongoRepository
{
    IFindFluent<History<T>, History<T>> Find<T>(Expression<Func<History<T>, bool>> filter);
    Task InsertOneAsync<T>(History<T> item);
    Task InsertManyAsync<T>(List<History<T>> item);
    Task ReplaceOneAsync<T>(string primaryRefId, History<T> item);

    Task<ReplaceManyResult> ReplaceManyAsync<T>(IEnumerable<History<T>> documents,
        Func<History<T>, FilterDefinition<History<T>>> filterExpression);

    Task DeleteOneAsync<T>(string primaryRefId);
    Task DeleteManyAsync<T>(string primaryRefId);
}
