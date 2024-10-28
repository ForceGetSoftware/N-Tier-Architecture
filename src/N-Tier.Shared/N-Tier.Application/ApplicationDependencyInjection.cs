using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using N_Tier.Application.Helpers;
using N_Tier.Application.MappingProfiles;
using N_Tier.DataAccess.Repositories;
using N_Tier.DataAccess.Repositories.Impl;
using N_Tier.Shared.Services;
using N_Tier.Shared.Services.Impl;

namespace N_Tier.Application;

public static class BaseApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.RegisterAutoMapper();

        return services;
    }

    private static IServiceCollection RegisterAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(IMappingProfilesMarker));
        return services;
    }

    public static IServiceCollection AddForcegetRabbitMq(this IServiceCollection services)
    {
        services.AddScoped<IForcegetRabbitMqManager, ForcegetRabbitMqManager>();
        return services;
    }

    public static IServiceCollection AddForcegetMongo(this IServiceCollection services)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        services.AddScoped<IBaseMongoRepository, BaseMongoRepository>();
        return services;
    }

    public static IServiceCollection AddClaimService(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddTransient<IClaimService, ClaimService>();
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
