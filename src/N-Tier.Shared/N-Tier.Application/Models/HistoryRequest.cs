using N_Tier.Shared.N_Tier.Application.Enums;

namespace N_Tier.Shared.N_Tier.Application.Models
{
    public class HistoryRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string RefId { get; set; }
        public string DatabaseName { get; set; }
        public string TableName { get; set; }
        public OrderBy OrderBy { get; set; } = OrderBy.Desc;
        public int? Take { get; set; }
        public int? Skip { get; set; }
    }
}
