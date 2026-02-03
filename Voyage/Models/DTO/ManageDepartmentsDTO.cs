namespace Voyage.Models.DTO
{
    public class ManageDepartmentsDTO
    {
        public ManageDepartmentsDTO()
        {
            Name = string.Empty;
        }

        public string DepartmentKey { get; set; }
        public int DepartmentId { get; set; }
        public string Name { get; set; }
    }
}
