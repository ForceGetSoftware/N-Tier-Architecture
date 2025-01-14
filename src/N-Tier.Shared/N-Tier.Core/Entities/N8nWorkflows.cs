using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using N_Tier.Core.Common;
using Plainquire.Filter.Abstractions;

namespace Auth.Core.Entities
{
    [Table("n8nworkflows", Schema = "system")]
    [EntityFilter(Prefix = "")]
    public class N8nWorkflows : ForcegetBaseEntity
    {
        [Required] public string code { get; set; }
        [Required] public string name { get; set; }
        [Required] public string url { get; set; }
    }
}
