using System.IdentityModel.Tokens.Jwt;
using N_Tier.DataAccess.Repositories.Impl;

namespace N_Tier.Shared.Services;

public interface IClaimService
{
    string GetUserId();
    Guid GetGuidUserId();
    string GetCompanyId();
    string GetClaim(string key);
    string GetAuthorization();
    Task<bool> IsSystemAdmin(IBaseRedisRepository baseRedisRepository);
    string GetRoleGroupTypeId();
    List<string> GetClaimList();
    JwtSecurityToken GetJwtToken();
}
