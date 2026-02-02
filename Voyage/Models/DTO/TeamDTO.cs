namespace Voyage.Models.DTO
{
    public class TeamDTO : BaseClass
    {
        public TeamDTO()
        {
        }

        public int EmployeeId { get; set; }
        public string TeamKey { get; set; }
        public int TeamId { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
        public int DepartmentId { get; set; }
        public int DbChangeAction { get; set; }

    }
}
