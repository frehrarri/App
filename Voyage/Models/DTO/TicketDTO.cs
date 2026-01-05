using Voyage.Utilities;
using static Voyage.Utilities.Constants;

namespace Voyage.Models.DTO
{
    public class TicketDTO : BaseClass
    {
        public TicketDTO() 
        {
            TicketDetailsDTOs = new List<TicketDetailsDTO>();
        }

        public int TicketId { get; set; }            
        public decimal TicketVersion { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = TicketStatus.NotStarted.ToString();
        public string Description { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public PriorityLevel PriorityLevel { get; set; } = PriorityLevel.Low;

        public DateTime? DueDate { get; set; }
        public int? ParentTicketId { get; set; }
        public string SectionTitle { get; set; } = string.Empty;

        public int SprintId { get; set; }
        public DateTime? SprintStartDate { get; set; }
        public DateTime? SprintEndDate { get; set; }

        public List<TicketDetailsDTO> TicketDetailsDTOs { get; set; }

    }
}
