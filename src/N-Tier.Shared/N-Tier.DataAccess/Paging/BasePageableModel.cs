using System.Text;

namespace N_Tier.Shared.N_Tier.DataAccess.Paging
{
    public abstract class BasePageableModel
    {
        public int Index { get; set; }
        public int Size { get; set; }
        public int Count { get; set; }
        public int Pages { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
    }
}
