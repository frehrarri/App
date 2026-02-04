namespace Voyage.Models.DTO
{
    public class AssignDepartmentDTO : BaseClass
    {
        public int DbChangeAction { get; set; }
        public string DepartmentKey { get; set; } = string.Empty;
        public string TeamKey { get; set; } = string.Empty;
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public int EmployeeId { get; set; }
        public int RoleId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
    }
}
