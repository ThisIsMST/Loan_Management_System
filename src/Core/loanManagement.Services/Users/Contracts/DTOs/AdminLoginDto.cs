namespace loanManagement.Services.Users.Contracts.DTOs
{
    public class AdminLoginDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}