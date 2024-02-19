using System.ComponentModel.DataAnnotations;
using Forceget.Enums;

namespace N_Tier.Core.Common;

public abstract class ForcegetBaseEntity : BaseEntity
{
    [Key]
    public int Id { get; set; }
    public Guid RefId { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public Guid? DeletedBy { get; set; }
    public DateTime? DeletedOn { get; set; }
    public EDataStatus DataStatus { get; set; } = EDataStatus.Active;
}
