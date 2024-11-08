namespace N_Tier.Shared.Services;

public interface IClaimService
{
    string GetUserId();
    Guid GetGuidUserId();
    string GetCompanyId();
    string GetClaim(string key);
    string GetAuthorization();
    bool IsSystemAdmin();
    string GetRoleGroupTypeId();
    List<string> GetClaimList();
}
