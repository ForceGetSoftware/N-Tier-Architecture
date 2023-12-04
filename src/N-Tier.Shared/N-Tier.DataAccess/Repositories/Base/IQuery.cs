namespace N_Tier.Shared.N_Tier.DataAccess.Repositories.Base
{
    public interface IQuery<T>
    {
        IQueryable<T> Query();
    }
}
