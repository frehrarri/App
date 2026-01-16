using Voyage.Models.DTO;

namespace Voyage.Models.App
{
    public class ManagePersonnelVM
    {
        public ManagePersonnelVM()
        {
            Personnel = new List<ManagePersonnelDTO>();
            //FirstName = string.Empty;
            //LastName = string.Empty;
            //Username = string.Empty;
            //Email = string.Empty;
            //PhoneNumber = string.Empty;
        }

        public List<ManagePersonnelDTO> Personnel {  get; set; }
        //public int EmployeeId { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        //public string Username { get; set; }
        //public string Email { get; set; }
        //public string PhoneNumber { get; set; }
    }
}
