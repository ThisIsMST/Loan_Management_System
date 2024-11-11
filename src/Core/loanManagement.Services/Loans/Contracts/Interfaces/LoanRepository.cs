using loanManagement.Services.Loans.Contracts.DTOs;
using LoanManagement.Entities.Loans;
using LoanManagement.Entities.Users;

namespace loanManagement.Services.Loans.Contracts.Interfaces
{
    public interface LoanRepository
    {
        int CalculateCreditScoreByAssetsAndLoan(decimal financialAssets, decimal loanAmount);
        int CalculateCreditScoreByCustomerJob(JobType jobType);
        int CalculateCreditScoreByCustomerMonthlyIncome(decimal monthlyIncome);
        int CalculateUserCreditScoreByPaymentHistory(CustomerBackgroundDto customerPaymentBackground);
        Loan? Find(int loanId);
        void Register(Loan loan);
        void Update(Loan loan);
        bool IsExistById(int loanId);
    }
}
