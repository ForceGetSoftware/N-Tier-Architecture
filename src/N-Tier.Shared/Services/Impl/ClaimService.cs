using Microsoft.AspNetCore.Http;
using N_Tier.DataAccess.Persistence;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace N_Tier.Shared.Services.Impl;

public class ClaimService(IHttpContextAccessor httpContextAccessor, ForcegetDatabaseContext context) : IClaimService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string GetUserId() => GetClaim(ClaimTypes.NameIdentifier);
    public string GetRoleGroupTypeId() => GetClaim("RoleGroupTypeId");

    public string GetCompanyId()
    {
        var companyId = GetHeader("CompanyId");
        if (string.IsNullOrEmpty(companyId))
        {
            companyId = GetHeader("Companyid");
        }

        return companyId;
    }

    public string GetClaim(string key) => _httpContextAccessor.HttpContext?.User?.FindFirst(key)?.Value;

    public string GetAuthorization() => _httpContextAccessor.HttpContext.Request.Headers.Authorization.ToString();

    public bool IsSystemAdmin()
    {
        try
        {
            const string bearerPrefix = "Bearer ";
            var authToken = GetAuthorization().Substring(bearerPrefix.Length);
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(authToken);
            var token = jsonToken as JwtSecurityToken;
            return token != null && token.Claims.Any(claim => claim.Value == "System Admin");
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public string GetHeader(string key) => _httpContextAccessor.HttpContext?.Request.Headers[key].ToString();

    public List<string> GetClaimList()
    {
        var bearerPrefix = "Bearer ";
        var authToken = GetAuthorization().Substring(bearerPrefix.Length);
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(authToken);
        return jsonToken is JwtSecurityToken token ? token.Claims.Select(x => x.Value).ToList() : [];
    }
}
