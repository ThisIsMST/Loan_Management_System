namespace loanManagement.Services.Loans.Contracts.DTOs
{
    public class CustomerBackgroundDto
    {
        public int OverdueInstallmentCount { get; set; }
        public int TotalLoansCount { get; set; }
        public int CustomerPendingLoanRequestCount { get; set; }
    }
}
