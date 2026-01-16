using Voyage.Models.DTO;

namespace Voyage.Models.App
{
    public class ManageRolesVM
    {
        public ManageRolesVM() 
        {
            Roles = new List<ManageRolesDTO>();
        }

        public List<ManageRolesDTO> Roles { get; set; }
    }
}
