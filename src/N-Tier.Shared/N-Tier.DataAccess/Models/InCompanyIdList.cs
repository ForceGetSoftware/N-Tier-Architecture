namespace N_Tier.Shared.N_Tier.DataAccess.Models;

public interface InCompanyRefIdList
{
    public Guid? companyrefid { get; set; }
}
public class InCompanyRefIdSumList : InCompanyRefIdList
{
    public Guid? companyrefid { get; set; }
    public double Value { get; set; }
}
