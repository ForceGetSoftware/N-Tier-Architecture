using Newtonsoft.Json;

namespace N_Tier.Application.Helpers;

public static class ForcegetHttpClient
{
    public static async Task<T> GetForceget<T>(this HttpClient httpClient, string url) where T : class
    {
        var response = await httpClient
        .GetAsync(url);
        var result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            var data = JsonConvert.DeserializeObject<T>(result,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            return data;
        }
        else if(response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
        {
            throw new Exception("GET - " + response.StatusCode + "-" + httpClient.BaseAddress.ToString() + "-" + url + "-" + result);
        }
        return null;
    }
    public static async Task<T> PostForceget<T>(this HttpClient httpClient, string url, List<KeyValuePair<string, string>> body) where T : class
    {
        var formContent = new FormUrlEncodedContent(body);

        var response = await httpClient
        .PostAsync(url, formContent);
        var result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            var data = JsonConvert.DeserializeObject<T>(result,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            return data;
        }
        else if(response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
        {
            throw new Exception("POST - " + response.StatusCode + "-" + httpClient.BaseAddress.ToString() + "-" + url + "-" + result);
        }
        return null;
    }

}