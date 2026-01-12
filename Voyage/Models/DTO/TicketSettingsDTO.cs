using Voyage.Utilities;
using static Voyage.Utilities.Constants;

namespace Voyage.Models.DTO
{
    public class TicketSettingsDTO
    {
        public TicketSettingsDTO()
        {
            //default to daily
            SprintStart = DateTime.Today;
            SprintEnd = DateTime.Today.AddDays(1);
            RepeatSprintOption = RepeatSprint.Daily;

            Sections = new List<SectionDTO>();
        }

        public Constants.RepeatSprint RepeatSprintOption { get; set; }
        public DateTime? SprintStart { get; set; }
        public DateTime? SprintEnd { get; set; }
        public List<SectionDTO> Sections { get; set; }
    }
}
