namespace N_Tier.Application.Models;

public class RabbitSendMailDto
{
    public string code { get; set; }
    public string email { get; set; }
    public string cc { get; set; }
    public string data { get; set; }
}
