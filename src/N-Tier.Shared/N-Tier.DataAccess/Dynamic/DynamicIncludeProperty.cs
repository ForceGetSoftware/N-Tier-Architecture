using System.Collections.Generic;

namespace N_Tier.Shared.N_Tier.DataAccess.Dynamic
{
    public class DynamicIncludeProperty
    {
        public DynamicQuery? Dynamic { get; set; }
        public List<string>? IncludeProperties { get; set; }

        public DynamicIncludeProperty()
        {
            IncludeProperties = new List<string>();
        }
    }
}
