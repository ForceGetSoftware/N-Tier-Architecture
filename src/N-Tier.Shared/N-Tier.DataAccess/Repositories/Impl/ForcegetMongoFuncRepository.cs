using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using N_Tier.Core.Entities;

namespace N_Tier.DataAccess.Repositories.Impl;

public class ForcegetMongoFuncRepository : IForcegetMongoFuncRepository
{
    private readonly IMongoDatabase _mongoDatabase;
    private readonly IMongoClient _mongoClient;
    
    public ForcegetMongoFuncRepository(IConfiguration configuration)
    {
        _mongoClient = new MongoClient(
            configuration["Mongo:ConnectionString"]);
        
        _mongoDatabase = _mongoClient.GetDatabase(
            configuration["Mongo:DatabaseName"]);
    }
    
    public IFindFluent<History<T>, History<T>> AsQuery<T>(Expression<Func<History<T>, bool>> filter)
    {
        var entityCollection = _mongoDatabase.GetCollection<History<T>>(
            typeof(T).Name);
        return entityCollection.Find(filter);
    }
    
    public async Task<List<History<T>>> GetAllAsync<T>()
    {
        return await AsQuery<T>(_ => true).ToListAsync();
    }
    
    public async Task<List<History<T>>> GetAllAsync<T>(Expression<Func<History<T>, bool>> filter)
    {
        return await AsQuery(filter).ToListAsync();
    }
    
    public async Task<History<T>?> GetAsync<T>(string primaryRefId)
    {
        var entityCollection = _mongoDatabase.GetCollection<History<T>>(
            typeof(T).Name);
        return await entityCollection.Find(x => x.PrimaryRefId == primaryRefId).FirstOrDefaultAsync();
    }
    
    public async Task<History<T>?> GetLatestAsync<T>(string primaryRefId)
    {
        var entityCollection = _mongoDatabase.GetCollection<History<T>>(
            typeof(T).Name);
        return await entityCollection
            .Find(x => x.PrimaryRefId == primaryRefId && x.CreationTime.AddDays(1) > DateTime.Now)
            .FirstOrDefaultAsync();
    }
    
    public async Task CreateAsync<T>(History<T> item)
    {
        var entityCollection = _mongoDatabase.GetCollection<History<T>>(
            typeof(T).Name);
        item.CreationTime = DateTime.Now;
        await entityCollection.InsertOneAsync(item);
    }
    
    public async Task UpdateAsync<T>(string primaryRefId, History<T> item)
    {
        var entityCollection = _mongoDatabase.GetCollection<History<T>>(
            typeof(T).Name);
        item.CreationTime = DateTime.Now;
        await entityCollection.ReplaceOneAsync(x => x.PrimaryRefId == primaryRefId, item);
    }
    
    public async Task UpdateAllAsync<T>(IEnumerable<History<T>> documents,
        Func<History<T>, FilterDefinition<History<T>>> filterExpression)
    {
        var entityCollection = _mongoDatabase.GetCollection<History<T>>(
            typeof(T).Name);
        
        using var session = await _mongoClient.StartSessionAsync();
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
            
            await entityCollection.BulkWriteAsync(session, bulkOps);
            await session.CommitTransactionAsync();
        }
        catch
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }
    
    public async Task RemoveAsync<T>(string primaryRefId)
    {
        var entityCollection = _mongoDatabase.GetCollection<History<T>>(
            typeof(T).Name);
        await entityCollection.DeleteOneAsync(x => x.PrimaryRefId == primaryRefId);
    }
    
    public async Task RemoveAllAsync<T>(Expression<Func<History<T>, bool>> filterExpression)
    {
        var entityCollection = _mongoDatabase.GetCollection<History<T>>(typeof(T).Name);
        await entityCollection.DeleteManyAsync(filterExpression);
    }
    
    public async Task SaveAsync<T>(string primaryRefId, T element)
    {
        var mongoItem = await AsQuery<T>(f => f.PrimaryRefId == primaryRefId).FirstOrDefaultAsync();
        if (mongoItem != null)
        {
            mongoItem.DbObject = element;
            await UpdateAsync(primaryRefId, mongoItem);
        }
        else
        {
            await CreateAsync(
                new History<T>
                {
                    PrimaryRefId = primaryRefId,
                    DbObject = element
                });
        }
    }
}
