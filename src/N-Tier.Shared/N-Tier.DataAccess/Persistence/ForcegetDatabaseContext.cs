using Microsoft.EntityFrameworkCore;
using N_Tier.Core.Common;
using N_Tier.Shared.Services;
using System.Reflection;

namespace N_Tier.DataAccess.Persistence;

public class ForcegetDatabaseContext(DbContextOptions options, IClaimService claimService) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<ForcegetBaseEntity>())
        {
            var userId = claimService.GetGuidUserId();
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.CreatedOn = DateTime.UtcNow.ToUniversalTime();
                    entry.Entity.DataStatus = Forceget.Enums.EDataStatus.Active;
                    break;
                case EntityState.Modified:
                    if (entry.Entity.DataStatus == Forceget.Enums.EDataStatus.Deleted)
                    {
                        entry.Entity.DeletedBy = userId;
                        entry.Entity.DeletedOn = DateTime.UtcNow.ToUniversalTime();
                    }
                    else
                    {
                        entry.Entity.UpdatedBy = userId;
                        entry.Entity.UpdatedOn = DateTime.UtcNow.ToUniversalTime();
                    }

                    break;
            }
        }

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
            {
                var property = entry.Entity.GetType().GetProperty("RefId");
                property?.SetValue(entry.Entity, Guid.NewGuid());
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
