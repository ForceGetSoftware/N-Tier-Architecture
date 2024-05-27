using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using N_Tier.Core.Entities;

namespace N_Tier.DataAccess.Repositories.Impl;

public class ForcegetMongoRepository<T> : IForcegetMongoRepository<T>
{
    private readonly IMongoCollection<History<T>> _entityCollection;
    
    public ForcegetMongoRepository(
        IConfiguration configuration)
    {
        var mongoClient = new MongoClient(
            configuration["Mongo:ConnectionString"]);
        
        var mongoDatabase = mongoClient.GetDatabase(
            configuration["Mongo:DatabaseName"]);
        
        _entityCollection = mongoDatabase.GetCollection<History<T>>(
            typeof(T).Name);
    }
    
    public IFindFluent<History<T>, History<T>> AsQuery(Expression<Func<History<T>, bool>> filter)
    {
        return _entityCollection.Find(filter);
    }
    
    public async Task<List<History<T>>> GetAllAsync()
    {
        return await AsQuery(_ => true).ToListAsync();
    }
    
    public async Task<List<History<T>>> GetAllAsync(Expression<Func<History<T>, bool>> filter)
    {
        return await AsQuery(filter).ToListAsync();
    }
    
    public async Task<History<T>?> GetAsync(string primaryRefId)
    {
        return await _entityCollection.Find(x => x.PrimaryRefId == primaryRefId).FirstOrDefaultAsync();
    }
    
    public async Task<History<T>?> GetLatestAsync(string primaryRefId)
    {
        return await _entityCollection
            .Find(x => x.PrimaryRefId == primaryRefId && x.CreationTime.AddDays(7) > DateTime.Now)
            .FirstOrDefaultAsync();
    }
    
    public async Task CreateAsync(History<T> item)
    {
        item.CreationTime = DateTime.Now;
        await _entityCollection.InsertOneAsync(item);
    }
    
    public async Task UpdateAsync(string primaryRefId, History<T> item)
    {
        item.CreationTime = DateTime.Now;
        await _entityCollection.ReplaceOneAsync(x => x.PrimaryRefId == primaryRefId, item);
    }
    
    public async Task RemoveAsync(string primaryRefId)
    {
        await _entityCollection.DeleteOneAsync(x => x.PrimaryRefId == primaryRefId);
    }
}
