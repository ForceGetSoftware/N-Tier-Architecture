﻿using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Auth.Core.Entities;
using N_Tier.DataAccess.Repositories.Impl;

namespace N_Tier.Shared.Services.Impl;

public class ClaimService(IHttpContextAccessor httpContextAccessor, IBaseRedisRepository baseRedisRepository)
    : IClaimService
{
    public string GetUserId()
    {
        try
        {
            return GetClaim("nameid");
        }
        catch (Exception e)
        {
            return string.Empty;
        }
    }

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
        var result = GetJwtToken().Claims.Where(claim => claim.Type == key).Select(s => s.Value).FirstOrDefault();
        if (!string.IsNullOrEmpty(result))
            return result;

        result = httpContextAccessor.HttpContext?.User?.FindFirstValue(key);

        if (!string.IsNullOrEmpty(result))
            return result;

        throw new Exception(key + " not found");
    }

    public string GetAuthorization() => httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();

    public JwtSecurityToken GetJwtToken()
    {
        var auth = GetAuthorization();
        if (string.IsNullOrEmpty(auth)) return new JwtSecurityToken();
        const string bearerPrefix = "Bearer ";
        var authToken = auth.Substring(bearerPrefix.Length);
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(authToken)) return new JwtSecurityToken();
        var jsonToken = handler.ReadToken(authToken);
        var token = jsonToken as JwtSecurityToken;
        return token;
    }

    public async Task<bool> IsSystemAdmin()
    {
        try
        {
            var userId = GetUserId();

            var roles = await baseRedisRepository.GetAsync<List<ForcegetRole>>(
                $"ForcegetUser_ForcegetRole_{userId}");
            return roles.Any(a => a.name == "Admin");
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
