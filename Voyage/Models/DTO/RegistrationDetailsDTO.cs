namespace Voyage.Models.DTO
{
    public class RegistrationDetailsDTO
    {
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public int PhoneCountryCode { get; set; }
        public int PhoneAreaCode { get; set; }
        public long Phone { get; set; }

        public string StreetAddress { get; set; } = string.Empty;
        public int UnitNumber { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public int ZipCode { get; set; }

        public CompanyDTO Company { get; set; } = new CompanyDTO();
    }
}
