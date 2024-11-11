using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace N_Tier.DataAccess.Repositories.Impl;

public class BaseRedisRepository(IDistributedCache distributedCache) : IBaseRedisRepository
{
    public async Task<string> GetStringAsync(string key)
    {
        return await distributedCache.GetStringAsync(key);
    }

    public async Task<T> GetAsync<T>(string key) where T : class
    {
        var result = await distributedCache.GetStringAsync(key);
        if(string.IsNullOrEmpty(result)) throw new KeyNotFoundException();
        return JsonSerializer.Deserialize<T>(result);
    }

    public Task SetStringAsync(string key, string value, TimeSpan? absoluteExpirationRelativeToNow)
    {
        return distributedCache
            .SetStringAsync(key, value,
            new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow });
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow) where T : class
    {
        return distributedCache
            .SetStringAsync(key, JsonSerializer.Serialize(value),
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
