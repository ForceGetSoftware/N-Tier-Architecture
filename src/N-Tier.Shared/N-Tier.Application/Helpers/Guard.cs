using System.Text;
using System.Text.Json;
using Auth.Core.Entities;

namespace N_Tier.Shared.N_Tier.Application.Helpers;

public static class Guard
{
    public static void Against<TException>(bool condition, string message) where TException : Exception, new()
    {
        if (condition)
        {
            var exception = (TException)Activator.CreateInstance(typeof(TException), message);
            throw exception;
        }
    }

    public static async Task<string> N8N(N8nWorkflows dbitem, IHttpClientFactory httpClientFactory, dynamic body)
    {
        if (dbitem == null) return null;
        var client = httpClientFactory.CreateClient();
        var response =
            await client.PostAsync(dbitem.url, new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"));
        if (!response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }
        return null;
    }
}
