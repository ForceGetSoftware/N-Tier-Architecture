using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace N_Tier.Shared.Services.Impl;

public class ClaimService(IHttpContextAccessor httpContextAccessor) : IClaimService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string GetUserId() => GetClaim(ClaimTypes.NameIdentifier);
    public string GetRoleGroupTypeId() => GetClaim("RoleGroupTypeId");

    public string GetCompanyId() => GetHeader("CompanyId");

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
}
