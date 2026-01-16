using Voyage.Models.DTO;

namespace Voyage.Models.App
{
    public class ManageTeamsVM
    {
        public ManageTeamsVM()
        {
            Teams = new List<ManageTeamsDTO>();
        }

        public List<ManageTeamsDTO> Teams { get; set; }
    }
}
