using Voyage.Models.DTO;

namespace Voyage.Models.App
{
    public class ManagePermissionsVM
    {
        public ManagePermissionsVM()
        {
            Permissions = new List<ManagePermissionsDTO>();
        }

        public List<ManagePermissionsDTO> Permissions { get; set; }
    }
}
