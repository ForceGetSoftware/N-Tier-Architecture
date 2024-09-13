using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using N_Tier.Application.Models;
using N_Tier.Core.Entities;

namespace N_Tier.DataAccess.Repositories.Impl;

public class BaseMongoRepository : IBaseMongoRepository
{
    private readonly IMongoDatabase _mongoDatabase;
    private readonly IMongoClient _mongoClient;

    public BaseMongoRepository(IConfiguration configuration)
    {
        _mongoClient = new MongoClient(
            configuration["Mongo:ConnectionString"]);

        _mongoDatabase = _mongoClient.GetDatabase(
            configuration["Mongo:DatabaseName"]);
    }

    public IFindFluent<History<T>, History<T>> Find<T>(Expression<Func<History<T>, bool>> filter)
    {
        var entityCollection = _mongoDatabase.GetCollection<History<T>>(
            typeof(T).Name);
        return entityCollection.Find(filter);
    }

    public async Task InsertOneAsync<T>(History<T> item)
    {
        var entityCollection = _mongoDatabase.GetCollection<History<T>>(
            typeof(T).Name);
        item.CreationTime = DateTime.Now;
        await entityCollection.InsertOneAsync(item);
    }

    public async Task InsertManyAsync<T>(List<History<T>> item)
    {
        var entityCollection = _mongoDatabase.GetCollection<History<T>>(
            typeof(T).Name);
        await entityCollection.InsertManyAsync(item);
    }

    public async Task ReplaceOneAsync<T>(string primaryRefId, History<T> item)
    {
        var entityCollection = _mongoDatabase.GetCollection<History<T>>(
            typeof(T).Name);
        item.CreationTime = DateTime.Now;
        await entityCollection.ReplaceOneAsync(x => x.PrimaryRefId == primaryRefId, item);
    }

    public async Task<ReplaceManyResult> ReplaceManyAsync<T>(IEnumerable<History<T>> documents,
        Func<History<T>, FilterDefinition<History<T>>> filterExpression)
    {
        var entityCollection = _mongoDatabase.GetCollection<History<T>>(
            typeof(T).Name);

        using (var session = await _mongoClient.StartSessionAsync())
        {
            session.StartTransaction();

            try
            {
                var bulkOps = new List<WriteModel<History<T>>>();

                foreach (var document in documents)
                {
                    var filter = filterExpression(document);
                    var replaceOne = new ReplaceOneModel<History<T>>(filter, document)
                    {
                        IsUpsert = true
                    };
                    bulkOps.Add(replaceOne);
                }

                var bulkResult =
                    await entityCollection.BulkWriteAsync(bulkOps, new BulkWriteOptions { IsOrdered = false });

                await session.CommitTransactionAsync();

                return new ReplaceManyResult(bulkResult.MatchedCount, bulkResult.ModifiedCount,
                    bulkResult.Upserts.Count);
            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync();
                throw new Exception("An error occurred while replacing documents", ex);
            }
        }
    }

    public async Task DeleteOneAsync<T>(string primaryRefId)
    {
        var entityCollection = _mongoDatabase.GetCollection<History<T>>(
            typeof(T).Name);
        await entityCollection.DeleteOneAsync(x => x.PrimaryRefId == primaryRefId);
    }

    public async Task DeleteManyAsync<T>(string primaryRefId)
    {
        var entityCollection = _mongoDatabase.GetCollection<History<T>>(
            typeof(T).Name);
        await entityCollection.DeleteManyAsync(x => x.PrimaryRefId == primaryRefId);
    }
}
