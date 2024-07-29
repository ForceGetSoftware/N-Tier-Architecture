using System.Linq.Expressions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using N_Tier.Core.Entities;

namespace N_Tier.DataAccess.Repositories.Impl;

public class BaseRedisRepository(IDistributedCache distributedCache) : IBaseRedisRepository
{
    public async Task<string> GetStringAsync(string key)
    {
        return await distributedCache.GetStringAsync(key);
    }

    public Task SetStringAsync(string key, string value, TimeSpan? absoluteExpirationRelativeToNow)
    {
        return distributedCache
            .SetStringAsync(key, value,
            new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow });
    }

    public Task RemoveAsync(string key)
    {
        return distributedCache
            .RemoveAsync(key);
    }

    public Task RefreshAsync(string key)
    {
        return distributedCache
            .RefreshAsync(key);
    }
}
