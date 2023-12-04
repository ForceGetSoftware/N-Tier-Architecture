using System.Collections.Generic;

namespace N_Tier.Shared.N_Tier.DataAccess.Dynamic
{
    public class IncludeProperty
    {
        public List<string> IncludeProperties { get; set; }

        public IncludeProperty()
        {
            IncludeProperties = new List<string>();
        }
    }
}
