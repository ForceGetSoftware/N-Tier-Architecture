using Microsoft.EntityFrameworkCore;
using N_Tier.Core.Common;
using N_Tier.Shared.Services;
using System.Reflection;
using Auth.Core.Entities;

namespace N_Tier.DataAccess.Persistence;

public class ForcegetDatabaseContext(DbContextOptions options, IClaimService claimService) : DbContext(options)
{
    public DbSet<N8nWorkflows> N8nWorkflows { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
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
                    entry.Entity.createdby = userId;
                    entry.Entity.createdon = DateTime.UtcNow.ToUniversalTime();
                    entry.Entity.datastatus = Forceget.Enums.EDataStatus.Active;
                    break;
                case EntityState.Modified:
                    if (entry.Entity.datastatus == Forceget.Enums.EDataStatus.Deleted)
                    {
                        entry.Entity.deletedby = userId;
                        entry.Entity.deletedon = DateTime.UtcNow.ToUniversalTime();
                    }
                    else
                    {
                        entry.Entity.updatedby = userId;
                        entry.Entity.updatedon = DateTime.UtcNow.ToUniversalTime();
                    }

                    break;
            }
        }

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
            {
                var property = entry.Entity.GetType().GetProperty("refid");
                property?.SetValue(entry.Entity, Guid.NewGuid());
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
