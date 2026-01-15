namespace Voyage.Models.DTO
{
    public class PasswordResetDTO
    {
        public string? Token { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
