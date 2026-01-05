
using static System.Collections.Specialized.BitVector32;

namespace Voyage.Models.App
{
    public class TicketsVM : BaseClass
    {
        public TicketsVM() 
        {
            Sections = new List<Section>();
            Tickets = new List<TicketVM>(); 
            Sprint = new Sprint();
        }

        public TicketsVM(List<TicketVM> tickets, List<Section> sections) 
        {
            Sections = sections;
            Tickets = tickets;
            Sprint = new Sprint();
        }

        public Sprint Sprint { get; set; }
        public List<TicketVM> Tickets { get; set; }
        public List<Section> Sections { get; set; }

   

    }
}
