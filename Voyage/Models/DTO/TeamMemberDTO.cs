namespace Voyage.Models.DTO
{
    public class TeamMemberDTO
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public int EmployeeId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Username { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;

    }
}
