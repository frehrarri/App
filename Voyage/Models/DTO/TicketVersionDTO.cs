namespace Voyage.Models.DTO
{
    public class TicketVersionDTO
    {
        public decimal TicketVersion { get; set; }
        public string TicketChangeAction { get; set; } = string.Empty;
    }
}
