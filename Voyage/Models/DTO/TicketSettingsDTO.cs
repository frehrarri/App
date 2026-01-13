using Voyage.Utilities;
using static Voyage.Utilities.Constants;

namespace Voyage.Models.DTO
{
    public class TicketSettingsDTO : BaseClass
    {
        public TicketSettingsDTO()
        {
            SprintStart = DateTime.Today.ToUniversalTime();
            SprintEnd = null;
            RepeatSprintOption = RepeatSprint.Never;

            Sections = new List<SectionDTO>();
        }

        public int SettingsId { get; set; }
        public Constants.RepeatSprint RepeatSprintOption { get; set; }
        public int SprintId { get; set; }
        public DateTime? SprintStart { get; set; }
        public DateTime? SprintEnd { get; set; }
        public List<SectionDTO> Sections { get; set; }
        public SectionSettings SectionSetting { get; set; }
    }
}
