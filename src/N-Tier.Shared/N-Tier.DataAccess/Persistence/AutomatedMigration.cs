using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySql.EntityFrameworkCore.Extensions;

namespace N_Tier.DataAccess.Persistence;

public static class AutomatedMigration
{
    public static async Task MigrateAsync<T>(IServiceProvider services) where T : DbContext
    {
        var context = services.GetRequiredService<T>();

        if (context.Database.IsMySql()) await context.Database.MigrateAsync();

        //var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    }
}
