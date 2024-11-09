using Plainquire.Filter;

namespace N_Tier.Shared.N_Tier.Core.Common;

public class GetAllRequest<TEntity>
{
    public EntityFilter<TEntity> Filter { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; }
    public string Search { get; set; }
    public string OrderBy { get; set; }
}
