namespace Voyage.Models.DTO
{
    public class TeamDTO
    {
        public TeamDTO()
        {
            UserEntityId = string.Empty;
            Name = string.Empty;
        }

        public int EmployeeId { get; set; }
        public string UserEntityId { get; set; }
        public string TeamKey { get; set; }
        public int TeamId { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
        public int DepartmentId { get; set; }
    }
}
