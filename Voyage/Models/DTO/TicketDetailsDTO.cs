using Voyage.Data.TableModels;

namespace Voyage.Models.DTO
{
    public class TicketDetailsDTO : BaseClass
    {
        public int? TicketDetailsId { get; set; }
        public int? TicketId { get; set; }
        public decimal? TicketVersion { get; set; }
        public string Note { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
    }
}
