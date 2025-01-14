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

    public static async void N8N<TException>(N8nWorkflows dbitem, IHttpClientFactory httpClientFactory, dynamic body)
        where TException : Exception, new()
    {
        if (dbitem != null)
        {
            var client = httpClientFactory.CreateClient();
            var response =
                await client.PostAsync(dbitem.url, new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
            {
                var exception =
                    (TException)Activator.CreateInstance(typeof(TException),
                        await response.Content.ReadAsStringAsync());
                throw exception;
            }
        }
    }
}
