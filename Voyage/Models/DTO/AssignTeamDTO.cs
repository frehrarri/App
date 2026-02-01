namespace Voyage.Models.DTO
{
    public class AssignTeamDTO
    {
        public AssignTeamDTO()
        {
            AvailableUsers = new List<UserDTO>();
            AssignedUsers = new List<UserDTO>();
        }

        public List<UserDTO> AvailableUsers { get; set; }
        public List<UserDTO> AssignedUsers { get; set; }

        public string TeamKey { get; set; }
        public int SaveAction { get; set; }
        public int CompanyId { get; set; }
        public int EmployeeId { get; set; }
        public int RoleId { get; set; }
    }
}
