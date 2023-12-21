using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace N_Tier.Application.Helpers;

public interface IForcegetRabbitMqManager
{
    bool Send(string queue, string message);
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
}
