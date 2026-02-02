using Voyage.Models.DTO;

namespace Voyage.Models.App
{
    public class ManageTeamsVM
    {
        public ManageTeamsVM()
        {
            Teams = new List<TeamDTO>();
        }

        public List<TeamDTO> Teams { get; set; }
    }
}
