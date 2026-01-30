namespace Voyage.Models.DTO
{
    public class UserDTO
    {
        public int CompanyId { get; set; }
        public int EmployeeId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public int PhoneAreaCode { get; set; }
        public int PhoneCountryCode { get; set; }
        public string Country { get; set; } = string.Empty;
        public string StreetAddress { get; set; } = string.Empty;
        public int UnitNumber { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public int PostalCode { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;

    }
}
