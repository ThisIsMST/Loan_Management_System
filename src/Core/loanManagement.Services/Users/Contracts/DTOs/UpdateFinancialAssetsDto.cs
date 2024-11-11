namespace loanManagement.Services.Users.Contracts.DTOs
{
    public class UpdateFinancialAssetsDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public decimal NewAssets { get; set; }
    }
}