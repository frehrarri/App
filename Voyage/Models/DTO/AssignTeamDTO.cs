namespace Voyage.Models.DTO
{
    public class AssignTeamDTO
    {
        public AssignTeamDTO()
        {
        }

        public int SaveAction { get; set; }
        public int EmployeeId { get; set; }
        public int RoleId { get; set; }
        public string Role { get; set; } = string.Empty;
        public int TeamId { get; set; }
        public string TeamKey { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
