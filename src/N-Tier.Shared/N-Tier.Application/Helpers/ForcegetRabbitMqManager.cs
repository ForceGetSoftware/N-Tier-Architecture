using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using N_Tier.Application.Models;
using RabbitMQ.Client;

namespace N_Tier.Application.Helpers;

public interface IForcegetRabbitMqManager
{
    Task<bool> Send(string queue, string message);
    Task<bool> SendMail(RabbitSendMailDto data);
}

public class ForcegetRabbitMqManager(IConfiguration configuration, IHttpClientFactory clientFactory)
    : IForcegetRabbitMqManager
{
    public async Task<bool> Send(string queue, string message)
    {
        var factory = new ConnectionFactory { HostName = configuration["RabbitMq"] };
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue,
            false,
            false,
            false,
            null);

        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(string.Empty, queue, body: body);

        return true;
    }

    public async Task<bool> SendMail(RabbitSendMailDto data)
    {
        var client = clientFactory.CreateClient("NotificationApi");
        var request = await client.PostAsJsonAsync("/notification/api/Email/Send", data);
        return request.IsSuccessStatusCode;

        var factory = new ConnectionFactory { HostName = configuration["RabbitMq"] };
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        var queue = "mail";
        await channel.QueueDeclareAsync(queue,
            false,
            false,
            false,
            null);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));

        await channel.BasicPublishAsync(string.Empty, queue, body: body);


        return true;
    }
}
