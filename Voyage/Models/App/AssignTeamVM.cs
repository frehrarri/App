using Voyage.Models.DTO;

namespace Voyage.Models.App
{
    public class AssignTeamVM
    {
        public AssignTeamVM() 
        {
            AvailableUsers = new List<UserVM>();
            AssignedUsers = new List<UserVM>();
        }

        public List<UserVM> AvailableUsers { get; set; }
        public List<UserVM> AssignedUsers { get; set; }
    }
}
