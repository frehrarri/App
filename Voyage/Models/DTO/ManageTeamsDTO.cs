using Voyage.Models.DTO;

namespace Voyage.Models.DTO
{
    public class ManageTeamsDTO
    {
        public ManageTeamsDTO()
        {
            Name = string.Empty;
            TeamMembers = new List<TeamMemberDTO>();
            Teams = new List<TeamDTO>();
        }

        public int TeamId { get; set; }
        public string Name { get; set; }

        public List<TeamMemberDTO> TeamMembers { get; set; }
        public List<TeamDTO> Teams { get; set; }
    }
}
