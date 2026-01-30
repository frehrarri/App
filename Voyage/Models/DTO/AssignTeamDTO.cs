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
    }
}
