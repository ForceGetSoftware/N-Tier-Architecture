# How to migrate ForcegetApplicationBase.Core
## Version 8.2.x > 8.2.5157

- Remove line `services.AddScoped<IForcegetMongoFuncRepository, ForcegetMongoFuncRepository>();`
- Replace All Files in  ``IForcegetMongoFuncRepository`` to `IBaseMongoRepository`
- Remove line `services.AddScoped<IClaimService, ClaimService>();`
- Remove line `builder.WebHost.UseForcegetSentry();`
- Remove line `.AddForcegetBaseLogging(builder.Configuration, "xxx")`
- Remove line `app.UseMiddleware<SerilogMiddleware>();`
- Replace `.AddForcegetRabbitMQ()` to `AddForcegetRabbitMq()`
- Check line if not like this add missing methods
  ```
  builder.Services
      .AddClaimService()
      .AddDataAccess(builder.Configuration)
      .AddApplication()
      .AddServices()
      .RegisterProfiles()
      .AddForcegetRabbitMq()
      .AddForcegetMongo()
      .AddForcegetRedisCache(builder.Configuration["Redis"]);
  ```

## Version 8.2.x > 8.2.8.2.5806

- Replace All Files in  ``using FS.FilterExpressionCreator.Filters;`` to `using Plainquire.Filter;`
- Remove line `builder.Services.AddJwt(builder.Configuration);`
- Remove line `app.UseAuthentication();`
- Remove line `app.UseAuthorization();`
- Remove line `[Authorize]`