using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using N_Tier.Core.Common;
using Plainquire.Filter.Abstractions;

namespace Auth.Core.Entities
{
    [Table("roles", Schema = "auth")]
    [Index(nameof(name), IsUnique = true)]
    [EntityFilter(Prefix = "")]
    public class ForcegetRole : ForcegetBaseEntity
    {
        [Required] public string name { get; set; }
        public string definition { get; set; }
    }
}