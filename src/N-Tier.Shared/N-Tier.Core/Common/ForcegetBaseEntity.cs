using System.ComponentModel.DataAnnotations;
using Forceget.Enums;
using Plainquire.Filter.Abstractions;

namespace N_Tier.Core.Common;

[EntityFilter(Prefix = "")]
public abstract class ForcegetBaseEntity : BaseEntity
{
    [Key] public int id { get; set; }
    
    public Guid refid { get; set; }
    public Guid createdby { get; set; }
    public DateTime createdon { get; set; }
    public Guid? updatedby { get; set; }
    public DateTime? updatedon { get; set; }
    public Guid? deletedby { get; set; }
    public DateTime? deletedon { get; set; }
    public EDataStatus datastatus { get; set; } = EDataStatus.Active;
}
