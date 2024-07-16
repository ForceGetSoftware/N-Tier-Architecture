namespace N_Tier.Shared.Services;

public interface IClaimService
{
    string GetUserId();
    string GetCompanyId();
    string GetClaim(string key);
    string GetAuthorization();
    bool IsSystemAdmin();
    string GetRoleGroupTypeId();
}
