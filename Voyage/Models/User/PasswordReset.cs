namespace Voyage.Models.User
{
    public class PasswordReset
    {
        public string? Token { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
