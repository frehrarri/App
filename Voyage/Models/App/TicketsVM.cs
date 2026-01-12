
using Voyage.Models.DTO;

namespace Voyage.Models.App
{
    public class TicketsVM : BaseClass
    {
        public TicketsVM() 
        {
            Sections = new List<SectionDTO>();
            Tickets = new List<TicketVM>(); 
            Sprint = new Sprint();
            Settings = new TicketSettingsDTO();
        }

        public TicketsVM(List<TicketVM> tickets, List<SectionDTO> sections) 
        {
            Sections = sections;
            Tickets = tickets;
            Sprint = new Sprint();
            Settings = new TicketSettingsDTO();
        }

        public Sprint Sprint { get; set; }
        public List<TicketVM> Tickets { get; set; }
        public List<SectionDTO> Sections { get; set; }
        public TicketSettingsDTO Settings { get; set; }
   

    }
}
