using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using N_Tier.Application.Models;
using RabbitMQ.Client;

namespace N_Tier.Application.Helpers;

public interface IForcegetRabbitMqManager
{
    bool Send(string queue, string message);
    bool SendMail(RabbitSendMailDto data);
}

public class ForcegetRabbitMqManager : IForcegetRabbitMqManager
{
    private readonly IConfiguration configuration;
    
    public ForcegetRabbitMqManager(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    
    public bool Send(string queue, string message)
    {
        var factory = new ConnectionFactory { HostName = configuration["RabbitMq"] };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        
        channel.QueueDeclare(queue,
            false,
            false,
            false,
            null);
        
        var body = Encoding.UTF8.GetBytes(message);
        
        channel.BasicPublish(string.Empty,
            queue,
            null,
            body);
        
        return true;
    }
    
    public bool SendMail(RabbitSendMailDto data)
    {
        var factory = new ConnectionFactory { HostName = configuration["RabbitMq"] };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        const string queue = "mail";
        channel.QueueDeclare(queue,
            false,
            false,
            false,
            null);
        
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
        
        channel.BasicPublish(string.Empty,
            queue,
            null,
            body);
        
        return true;
    }
}
