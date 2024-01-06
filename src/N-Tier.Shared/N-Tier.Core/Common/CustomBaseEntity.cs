using System.ComponentModel.DataAnnotations;
using Forceget.Enums;

namespace N_Tier.Core.Common;

public abstract class CustomBaseEntity
{
    [Key]
    public int Id { get; set; }
    [MaxLength(36)]
    public Guid RefId { get; set; }
    [MaxLength(36)]
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    [MaxLength(36)]
    public string UpdatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
    [MaxLength(36)]
    public string DeletedBy { get; set; }
    public DateTime? DeletedOn { get; set; }
    public EuDataStatus DataStatus { get; set; } = EuDataStatus.Active;
}