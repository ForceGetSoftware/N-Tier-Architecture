using FluentValidation;
using FluentValidation.AspNetCore;
using FS.FilterExpressionCreator.Mvc.Extensions;
using FS.FilterExpressionCreator.Swashbuckle.Extensions;
using Microsoft.Extensions.DependencyInjection;
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
        ).AddFilterExpressionSupport();
        
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
            
            s.AddFilterExpressionSupport();
            
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
