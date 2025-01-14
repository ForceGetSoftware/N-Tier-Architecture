using System.Text;
using Forceget.Enums;
using N_Tier.Shared.N_Tier.DataAccess.Repositories;

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

    public static async void N8N<TException>(IN8nWorkflowsRepository repository, IHttpClientFactory httpClientFactory, string code, dynamic body)
        where TException : Exception, new()
    {
        var dbitem = await repository.GetFirstOrDefaultAsync(f=>f.datastatus == EDataStatus.Active && f.code == code);
        if (dbitem != null)
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.PostAsync(dbitem.url, new StringContent(body, Encoding.UTF8, "application/json"));
            if(!response.IsSuccessStatusCode)
            {
                var exception = (TException)Activator.CreateInstance(typeof(TException), await response.Content.ReadAsStringAsync());
                throw exception;
            }
        }
    }
}
