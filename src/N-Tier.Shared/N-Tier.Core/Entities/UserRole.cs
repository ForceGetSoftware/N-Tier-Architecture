using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Auth.Core.Enums;
using N_Tier.Core.Common;
using Plainquire.Filter.Abstractions;

namespace Auth.Core.Entities
{
    [Table("userroles", Schema = "auth")]
    [EntityFilter(Prefix = "")]
    public class UserRole : ForcegetBaseEntity
    {
        [Required] public int roleid { get; set; }
        [Required] public int userid { get; set; }
        [Required] public ECompanyRoleType companyroletype { get; set; } = ECompanyRoleType.Team;
    }
}