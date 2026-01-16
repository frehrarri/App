using Voyage.Models.DTO;

namespace Voyage.Models.App
{
    public class ManageDepartmentsVM
    {
        public ManageDepartmentsVM()
        {
            Departments = new List<ManageDepartmentsDTO>();
        }

        public List<ManageDepartmentsDTO> Departments { get; set; }
    }
}
