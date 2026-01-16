namespace Voyage.Models.DTO
{
    public class ManagePersonnelDTO
    {
        public ManagePersonnelDTO()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Username = string.Empty;
            Email = string.Empty;
            PhoneNumber = string.Empty;
        }

        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
