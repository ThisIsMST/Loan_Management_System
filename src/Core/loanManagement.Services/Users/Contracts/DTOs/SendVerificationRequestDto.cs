namespace loanManagement.Services.Users.Contracts.DTOs
{
    public class SendVerificationRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}