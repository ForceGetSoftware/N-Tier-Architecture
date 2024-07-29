using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using N_Tier.Application.Services;
using N_Tier.Application.Services.DevImpl;
using N_Tier.Application.Services.Impl;
using N_Tier.Shared.Services;
using N_Tier.Shared.Services.Impl;

namespace N_Tier.Application;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddScoped<IWeatherForecastService, WeatherForecastService>();
        services.AddScoped<ITodoListService, TodoListService>();
        services.AddScoped<ITodoItemService, TodoItemService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IClaimService, ClaimService>();
        services.AddScoped<ITemplateService, TemplateService>();

        if (env.IsDevelopment())
            services.AddScoped<IEmailService, DevEmailService>();
        return services;
    }
}
