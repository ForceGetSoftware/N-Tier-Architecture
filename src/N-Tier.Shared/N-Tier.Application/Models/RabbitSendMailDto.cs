namespace N_Tier.Application.Models;

public class RabbitSendMailDto
{
    public string code { get; set; }
    public string email { get; set; }
    public string cc { get; set; }
    public string data { get; set; }
    public List<RabbitMqAttachment>? attachment { get; set; }
}

public class RabbitMqAttachment
{
    public string url { get; set; }
    public string token { get; set; }
    public string name { get; set; }
    public string contenttype { get; set; }
}