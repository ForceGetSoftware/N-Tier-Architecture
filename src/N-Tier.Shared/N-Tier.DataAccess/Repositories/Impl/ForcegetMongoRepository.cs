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

    public IFindFluent<History<T>, History<T>> AsQuery(Expression<Func<History<T>, bool>> filter) =>
        _entityCollection.Find(filter);
    
    public async Task<List<History<T>>> GetAllAsync() =>
        await AsQuery(_ => true).ToListAsync();
    
    public async Task<List<History<T>>> GetAllAsync(Expression<Func<History<T>, bool>> filter) =>
        await AsQuery(filter).ToListAsync();

    public async Task<History<T?>> GetAsync(string primaryRefId) =>
        await _entityCollection.Find(x => x.PrimaryRefId == primaryRefId).FirstOrDefaultAsync();

    public async Task CreateAsync(History<T> item) =>
        await _entityCollection.InsertOneAsync(item);

    public async Task UpdateAsync(string primaryRefId, History<T> item) =>
        await _entityCollection.ReplaceOneAsync(x => x.PrimaryRefId == primaryRefId, item);

    public async Task RemoveAsync(string primaryRefId) =>
        await _entityCollection.DeleteOneAsync(x => x.PrimaryRefId == primaryRefId);
}
