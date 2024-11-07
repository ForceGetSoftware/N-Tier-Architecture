using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Forceget.Enums;

namespace N_Tier.Core.Common;

public abstract class ForcegetBaseEntity : BaseEntity
{
    [Column("id")]
    [Key] public int Id { get; set; }
    
    [Column("refid")]
    public Guid RefId { get; set; }
    [Column("createdby")]
    public Guid CreatedBy { get; set; }
    [Column("createdon")]
    public DateTime CreatedOn { get; set; }
    [Column("updatedby")]
    public Guid? UpdatedBy { get; set; }
    [Column("updatedon")]
    public DateTime? UpdatedOn { get; set; }
    [Column("deletedby")]
    public Guid? DeletedBy { get; set; }
    [Column("deletedon")]
    public DateTime? DeletedOn { get; set; }
    [Column("datastatus")]
    public EDataStatus DataStatus { get; set; } = EDataStatus.Active;
}
