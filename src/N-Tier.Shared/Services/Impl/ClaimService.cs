using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace N_Tier.Shared.Services.Impl;

public class ClaimService : IClaimService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public ClaimService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public string GetUserId()
    {
        return GetClaim(ClaimTypes.NameIdentifier);
    }
    
    public string GetClaim(string key)
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(key)?.Value;
    }
    
    public string GetAuthorization()
    {
        return _httpContextAccessor.HttpContext.Request.Headers.Authorization.ToString();
    }
    
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
}
