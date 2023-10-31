using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace N_Tier.DataAccess.Identity;

[Table("users")]
public class ApplicationUser : IdentityUser { }
