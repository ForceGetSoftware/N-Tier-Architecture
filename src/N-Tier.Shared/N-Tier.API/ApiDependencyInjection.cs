using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using FS.FilterExpressionCreator.Mvc.Extensions;
using FS.FilterExpressionCreator.Swashbuckle.Extensions;
using FS.SortQueryableCreator.Mvc.Extensions;
using FS.SortQueryableCreator.Swashbuckle.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using N_Tier.API.Filters;
using N_Tier.Application.Models.Validators;

namespace N_Tier.API;

public static class ApiDependencyInjection
{
    public static void AddForcegetControllers(this IServiceCollection services)
    {
        services.AddControllers(
            config => config.Filters.Add(typeof(ValidateModelAttribute))
        ).AddFilterExpressionSupport().AddSortQueryableSupport();
        
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining(typeof(IValidationsMarker));
    }
    
    public static void UseSentry(this IWebHostBuilder services)
    {
        // Add the following line:
        services.UseSentry(o =>
        {
            o.Dsn = "https://32d239b4ea70bf225394e6eb12cb4f76@o4507326304485376.ingest.us.sentry.io/4507326307762176";
            // When configuring for the first time, to see what the SDK is doing:
            o.Debug = true;
            // Set TracesSampleRate to 1.0 to capture 100%
            // of transactions for performance monitoring.
            // We recommend adjusting this value in production
            o.TracesSampleRate = 1.0;
            // Sample rate for profiling, applied on top of othe TracesSampleRate,
            // e.g. 0.2 means we want to profile 20 % of the captured transactions.
            // We recommend adjusting this value in production.
            o.ProfilesSampleRate = 1.0;
            // Requires NuGet package: Sentry.Profiling
            // Note: By default, the profiler is initialized asynchronously. This can
            // be tuned by passing a desired initialization timeout to the constructor.
        });
    }
    
    public static void AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var secretKey = configuration.GetValue<string>("JwtConfiguration:SecretKey");
        
        var key = Encoding.ASCII.GetBytes(secretKey);
        
        services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
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
            
            s.AddFilterExpressionSupport();
            
            s.AddSortQueryableSupport();
            
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
}
