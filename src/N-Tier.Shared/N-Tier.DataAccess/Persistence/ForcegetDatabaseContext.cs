using Microsoft.EntityFrameworkCore;
using N_Tier.Core.Common;
using N_Tier.Shared.Services;
using System.Reflection;

namespace N_Tier.DataAccess.Persistence;

public class ForcegetDatabaseContext(DbContextOptions options, IClaimService claimService) : DbContext(options)
{
    private readonly IClaimService _claimService = claimService;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<ForcegetBaseEntity>())
            switch (entry.State)
            {
                case EntityState.Added:
                    //entry.Entity.RefId = Guid.NewGuid();
                    entry.Entity.CreatedBy = Guid.Parse(_claimService.GetUserId());
                    entry.Entity.CreatedOn = DateTime.Now;
                    entry.Entity.DataStatus = Forceget.Enums.EDataStatus.Active;
                    break;
                case EntityState.Modified:
                    if (entry.Entity.DataStatus == Forceget.Enums.EDataStatus.Deleted)
                    {
                        entry.Entity.DeletedBy = Guid.Parse(_claimService.GetUserId());
                        entry.Entity.DeletedOn = DateTime.Now;
                    }
                    else
                    {
                        entry.Entity.UpdatedBy = Guid.Parse(_claimService.GetUserId());
                        entry.Entity.UpdatedOn = DateTime.Now;
                    }
                    break;
            }

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
            {
                var property = entry.Entity.GetType().GetProperty("RefId");
                property?.SetValue(entry.Entity, Guid.NewGuid().ToString());
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
