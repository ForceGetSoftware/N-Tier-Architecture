﻿using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using N_Tier.API.Filters;
using N_Tier.Application.Models.Validators;
using Plainquire.Filter.Mvc;
using Plainquire.Filter.Swashbuckle;

namespace N_Tier.API;

public static class ApiDependencyInjection
{
    public static void AddForcegetControllers(this IServiceCollection services)
    {
        services.AddControllers(
            config => config.Filters.Add(typeof(ValidateModelAttribute))
        ).AddFilterSupport();

        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining(typeof(IValidationsMarker));
    }


    public static void AddSwagger(this IServiceCollection services, string title, string version)
    {
        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("v1", new OpenApiInfo { Title = title, Version = version });
            s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer YOUR_TOKEN')",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            s.AddFilterSupport();

            s.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }


    public static void AddDatabase<TAppDatabaseContext>(this IServiceCollection services, IConfiguration configuration)
        where TAppDatabaseContext : DbContext
    {
        var databaseConfig = configuration.GetSection("Database").Get<DatabaseConfiguration>();

        if (databaseConfig.UseInMemoryDatabase)
        {
            services.AddDbContext<TAppDatabaseContext>(options =>
            {
                options.UseInMemoryDatabase("NTierDatabase");
                options.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });
            return;
        }

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.AddDbContextPool<TAppDatabaseContext>(options =>
            options.UseNpgsql(databaseConfig.ConnectionString).EnableDetailedErrors().EnableSensitiveDataLogging().EnableServiceProviderCaching());
    }
}
