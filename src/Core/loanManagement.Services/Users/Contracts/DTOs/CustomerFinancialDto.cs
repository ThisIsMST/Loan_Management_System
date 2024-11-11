using LoanManagement.Entities.Users;

namespace loanManagement.Services.Users.Contracts.DTOs
{
    public class CustomerFinancialDto
    {
        public decimal MonthlyIncome { get; set; }
        public JobType JobType { get; set; }
        public decimal FinancialAssets { get; set; }
    }
}
