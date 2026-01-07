using Voyage.Data.TableModels;
using Voyage.Models.DTO;
using static Voyage.Utilities.Constants;

namespace Voyage.Models.App
{
    public class TicketVM : BaseClass
    {
        public TicketVM() 
        {
            TicketDetails = new List<TicketDetailsDTO>();
        }

        public int TicketId { get; set; }
        public decimal? TicketVersion { get; set; }  
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public PriorityLevel PriorityLevel { get; set; } = PriorityLevel.Low;
        public DateTime? DueDate { get; set; } = null;
        public int? ParentTicketId { get; set; } = null;
        public string SectionTitle { get; set; } = string.Empty;
        public int SprintId { get; set; }
        public DateTime SprintStartDate { get; set; }
        public DateTime SprintEndDate { get; set; }
        public List<decimal> VersionHistory { get; set; }
        public List<TicketDetailsDTO> TicketDetails { get; set; }

    }
}
