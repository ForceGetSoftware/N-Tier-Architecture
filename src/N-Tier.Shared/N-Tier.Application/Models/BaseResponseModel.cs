namespace N_Tier.Application.Models;

public class BaseResponseModel
{
    public int Id { get; set; }
    public Guid RefId { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
}
