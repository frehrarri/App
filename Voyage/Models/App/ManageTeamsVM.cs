using Voyage.Models.DTO;

namespace Voyage.Models.App
{
    public class ManageTeamsVM
    {
        public ManageTeamsVM()
        {
            Teams = new List<ManageTeamsDTO>();
            ManageTeamsDTO = new ManageTeamsDTO();
        }


        public List<ManageTeamsDTO> Teams { get; set; }
        public ManageTeamsDTO ManageTeamsDTO { get; set; }
    }
}
