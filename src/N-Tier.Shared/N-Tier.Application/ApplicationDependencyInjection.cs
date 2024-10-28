using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using N_Tier.Application.Helpers;
using N_Tier.Application.MappingProfiles;
using N_Tier.DataAccess.Repositories.Impl;

namespace N_Tier.Application;

public static class BaseApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.RegisterAutoMapper();

        return services;
    }

    private static void RegisterAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(IMappingProfilesMarker));
    }

    public static IServiceCollection AddForcegetRabbitMQ(this IServiceCollection services)
    {
        services.AddScoped<IForcegetRabbitMqManager, ForcegetRabbitMqManager>();

        return services;
    }

    public static IServiceCollection AddForcegetRedisCache(this IServiceCollection services, string redisUrl)
    {
        services.AddStackExchangeRedisCache(options => { options.Configuration = redisUrl; });
        services.Add(ServiceDescriptor.Singleton<IDistributedCache, RedisCache>());
        services.AddScoped<IBaseRedisRepository, BaseRedisRepository>();
        return services;
    }
}
