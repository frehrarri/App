using Voyage.Data.TableModels;
using static Voyage.Utilities.Constants;

namespace Voyage.Models.App
{
    public class TicketVM : BaseClass
    {
        public TicketVM() { }


        public int TicketId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public string PriorityLevel { get; set; }
        public DateTime? DueDate { get; set; } = null;
        public int? ParentTicketId { get; set; } = null;
        public string SectionTitle { get; set; } = string.Empty;

    }
}
