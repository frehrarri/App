namespace Voyage.Models.DTO
{
    public class DepartmentDTO : BaseClass
    {
        public DepartmentDTO()
        {
            Teams = new List<TeamDTO>();
        }

        public string DeptKey { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public int CompanyId { get; set; }
        public int EmployeeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DbChangeAction { get; set; }

        public List<TeamDTO> Teams { get; set; }
    }
}
