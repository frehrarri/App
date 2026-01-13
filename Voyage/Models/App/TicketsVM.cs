
using Voyage.Models.DTO;

namespace Voyage.Models.App
{
    public class TicketsVM : BaseClass
    {
        public TicketsVM() 
        {
            Sections = new List<SectionDTO>();
            Tickets = new List<TicketVM>(); 
            Sprint = new SprintVM();
            Settings = new TicketSettingsDTO();
        }

        public TicketsVM(List<TicketVM> tickets, List<SectionDTO> sections) 
        {
            Sections = sections;
            Tickets = tickets;
            Sprint = new SprintVM();
            Settings = new TicketSettingsDTO();
        }

        public SprintVM Sprint { get; set; }
        public List<TicketVM> Tickets { get; set; }
        public List<SectionDTO> Sections { get; set; }
        public TicketSettingsDTO Settings { get; set; }
   

    }
}
