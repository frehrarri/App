using Voyage.Utilities;
using static Voyage.Utilities.Constants;

namespace Voyage.Models.DTO
{
    public class TicketSettingsDTO : BaseClass
    {
        public TicketSettingsDTO()
        {
            SprintStart = DateTime.Today.ToUniversalTime();
            RepeatSprintOption = (int)RepeatSprint.Never;
            Sections = new List<SectionDTO>();
            IsEndDatePassed = false;
            SettingsHistory = new List<TicketSettingsDTO>();
        }

        public int CompanyId { get; set; }
        public int EmployeeId { get; set; }
        public Guid? DepartmentKey { get; set; }
        public Guid? TeamKey { get; set; }
        public int SettingsId { get; set; }
        public decimal SettingsVersion { get; set; }
        public int RepeatSprintOption { get; set; }
        public int SprintId { get; set; }
        public DateTime? SprintStart { get; set; }
        public DateTime? SprintEnd { get; set; }
        public int SprintLength { get; set; }
        public List<SectionDTO> Sections { get; set; }
        public SectionSettings SectionSetting { get; set; }
        public bool IsEndDatePassed { get; set; }
        public List<TicketSettingsDTO> SettingsHistory { get; set; }
    }
}
