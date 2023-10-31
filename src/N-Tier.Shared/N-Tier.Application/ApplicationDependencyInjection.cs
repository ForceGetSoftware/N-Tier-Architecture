﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using N_Tier.Application.Helpers;
using N_Tier.Application.MappingProfiles;
using Serilog;

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
    public static IServiceCollection AddForcegetBaseLogging(this IServiceCollection services, IConfiguration Configuration, string projectName)
    {
        if (Configuration["Seq:ServerUrl"] != null)
        {
            Log.Logger = new LoggerConfiguration()
                   .Enrich.FromLogContext()
                   .Enrich.WithProperty("Environment.MachineName", Environment.MachineName)
                   .Enrich.WithProperty("Environment.OSVersion", Environment.OSVersion)
                   .Enrich.WithProperty("Environment.UserName", Environment.UserName)
                   .Enrich.WithProperty("Environment.UserDomainName", Environment.UserDomainName)
                   .Enrich.WithProperty("Environment.Version", Environment.Version)
                   .Enrich.WithProperty("Project", projectName)
                   .WriteTo.Seq(Configuration["Seq:ServerUrl"])
                   .CreateLogger();

            services.AddLogging(loggingBuilder => { loggingBuilder.AddSerilog(); });
        }
        return services;
    }

}
