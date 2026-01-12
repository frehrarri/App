using Voyage.Models.DTO;
using static Voyage.Utilities.Constants;

namespace Voyage.Models.App
{
    public class TicketSettingsVM
    {
        public TicketSettingsVM()
        {
            //default to daily
            SprintStart = DateTime.Today;
            SprintEnd = DateTime.Today.AddDays(1);
            RepeatSprintOption = (int)RepeatSprint.Daily;
            Sections = new List<SectionDTO>();
        }

        public int SettingsId { get; set; }
        public int RepeatSprintOption { get; set; }
        public DateTime SprintStart { get; set; }
        public DateTime SprintEnd { get; set; }
        public List<SectionDTO> Sections { get; set; }
    }
}
