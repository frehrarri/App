using Voyage.Models.DTO;
using static Voyage.Utilities.Constants;

namespace Voyage.Models.App
{
    public class TicketSettingsVM
    {
        public TicketSettingsVM()
        {
            SprintStart = DateTime.Today.ToUniversalTime();
            SprintEnd = null;
            RepeatSprintOption = (int)RepeatSprint.Never;

            Sections = new List<SectionDTO>();
        }

        public int SettingsId { get; set; }
        public int? RepeatSprintOption { get; set; }
        public DateTime SprintStart { get; set; }
        public DateTime? SprintEnd { get; set; }
        public List<SectionDTO> Sections { get; set; }
        public SectionSettings SectionSetting { get; set; }
    }
}
