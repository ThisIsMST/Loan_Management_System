namespace loanManagement.Services.Users.Contracts.DTOs
{
    public class ChangePasswordDto
    {
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordConfirmation { get; set; }
    }
}