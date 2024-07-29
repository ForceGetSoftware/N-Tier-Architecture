using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using N_Tier.Core.Entities;

namespace N_Tier.DataAccess.Repositories.Impl;

public class BaseMongoRepository : IBaseMongoRepository
{
    private readonly IMongoDatabase _mongoDatabase;
    
    public BaseMongoRepository(IConfiguration configuration)
    {
        var mongoClient = new MongoClient(
            configuration["Mongo:ConnectionString"]);
        
        _mongoDatabase = mongoClient.GetDatabase(
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
