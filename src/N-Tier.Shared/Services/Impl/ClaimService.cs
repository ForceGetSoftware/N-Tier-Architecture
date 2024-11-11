﻿using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace N_Tier.Shared.Services.Impl;

public class ClaimService(IHttpContextAccessor httpContextAccessor) : IClaimService
{
    public string GetUserId() => GetClaim("nameid");
    public Guid GetGuidUserId()
    {
        if (!Guid.TryParse(GetUserId(), out var userId))
            userId = Guid.Empty;
        return userId;
    }

    public string GetRoleGroupTypeId() => GetClaim("RoleGroupTypeId");

    public string GetCompanyId()
    {
        return GetHeader("CompanyId");
    }

    public string GetClaim(string key)
    {
        var result = httpContextAccessor.HttpContext?.User?.FindFirst(key)?.Value;
        if (result == null)
        {
            result =GetJwtToken().Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        return result;
    }

    public string GetAuthorization() => httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();

    public JwtSecurityToken GetJwtToken()
    {
        const string bearerPrefix = "Bearer ";
        var authToken = GetAuthorization().Substring(bearerPrefix.Length);
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(authToken);
        var token = jsonToken as JwtSecurityToken;
        return token;
    }
    public bool IsSystemAdmin()
    {
        try
        {
            var token = GetJwtToken();
            return token != null && token.Claims.Any(claim => claim.Value == "System Admin");
        }
        catch 
        {
            return false;
        }
    }

    public string GetHeader(string key) => httpContextAccessor.HttpContext?.Request.Headers[key].ToString();

    public List<string> GetClaimList()
    {
        var bearerPrefix = "Bearer ";
        var authToken = GetAuthorization().Substring(bearerPrefix.Length);
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(authToken);
        return jsonToken is JwtSecurityToken token ? token.Claims.Select(x => x.Value).ToList() : [];
    }
}
