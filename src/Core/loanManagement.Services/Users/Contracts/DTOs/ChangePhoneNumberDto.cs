namespace loanManagement.Services.Users.Contracts.DTOs
{
    public class ChangePhoneNumberDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string NewPhoneNumber { get; set; }
    }
}