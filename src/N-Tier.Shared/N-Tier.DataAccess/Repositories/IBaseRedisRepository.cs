namespace N_Tier.DataAccess.Repositories.Impl;

public interface IBaseRedisRepository
{
    public Task<string> GetStringAsync(string key);
    public Task SetStringAsync(string key, string value, TimeSpan? absoluteExpirationRelativeToNow);
    public Task RemoveAsync(string key);
    public Task RefreshAsync(string key);
}
