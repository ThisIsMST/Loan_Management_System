namespace loanManagement.Services.Users.Contracts.DTOs
{
    public class UpdateMonthlyIncomeDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public decimal NewIncome { get; set; }
    }
}