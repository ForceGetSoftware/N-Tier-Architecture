using Microsoft.AspNetCore.Identity;
using N_Tier.DataAccess.Identity;

namespace N_Tier.DataAccess.Persistence;

public interface IDatabaseContextSeed
{
    Task SeedDatabaseAsync(ForcegetDatabaseContext context, UserManager<ApplicationUser> userManager);
}
