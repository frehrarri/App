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
        }

        public int RepeatSprintOption { get; set; }
        public DateTime SprintStart { get; set; }
        public DateTime SprintEnd { get; set; }
    }
}
