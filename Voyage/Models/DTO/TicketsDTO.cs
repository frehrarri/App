namespace Voyage.Models.DTO
{
    public class TicketsDTO
    {
        public TicketsDTO()
        {
            Tickets = new List<TicketDTO>();
        }

        public List<TicketDTO> Tickets { get; set; }
        public int ResultCount { get; set; }
    }
}
