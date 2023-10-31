using Microsoft.EntityFrameworkCore;
using N_Tier.Core.Entities;
using N_Tier.Shared.Services;

namespace N_Tier.DataAccess.Persistence;

public class AppDatabaseContext : DatabaseContext
{
    private readonly IClaimService _claimService;

    public AppDatabaseContext(DbContextOptions options, IClaimService claimService) : base(options, claimService)
    {
        _claimService = claimService;
    }

    public DbSet<TodoItem> TodoItems {get;set;}
    public DbSet<TodoList> TodoLists {get;set;}
}
