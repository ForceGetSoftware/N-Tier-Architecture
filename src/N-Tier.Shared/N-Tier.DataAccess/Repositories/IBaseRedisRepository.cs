namespace N_Tier.DataAccess.Repositories.Impl;

public interface IBaseRedisRepository
{
    public Task<string> GetStringAsync(string key);
    Task<T> GetAsync<T>(string key) where T : class;
    public Task SetStringAsync(string key, string value, TimeSpan? absoluteExpirationRelativeToNow);
    Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow) where T : class;
    public Task RemoveAsync(string key);
    public Task RefreshAsync(string key);
}
