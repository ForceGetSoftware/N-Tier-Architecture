using Auth.Core.Entities;
using N_Tier.DataAccess.Persistence;
using N_Tier.Shared.N_Tier.DataAccess.Repositories;

namespace N_Tier.DataAccess.Repositories.Impl;

public class N8nWorkflowsRepository(ForcegetDatabaseContext context, IBaseMongoRepository mongoRepository)
    : BaseRepository<N8nWorkflows>(context, mongoRepository), IN8nWorkflowsRepository
{
}
