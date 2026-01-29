using Voyage.Models.DTO;

namespace Voyage.Models.App
{
    public class ManagePersonnelVM : UserDTO
    {
        public ManagePersonnelVM() 
        {
            Personnel = new List<ManagePersonnelDTO>();
            Roles = new List<ManageRolesDTO>();
        }

        public List<ManagePersonnelDTO> Personnel {  get; set; }
        public List<ManageRolesDTO> Roles { get; set; }

    }
}
