using loanManagement.Services;
using loanManagement.Services.Loans.Contracts.DTOs;
using loanManagement.Services.Loans.Contracts.Interfaces;
using LoanManagement.Entities.Loans;
using LoanManagement.Entities.Users;
using LoanManagement.Persistence.EF.DataContext;

namespace LoanManagement.Persistence.EF.Loans
{
    public class EFLoanRepository(EFDataContext context) : LoanRepository
    {
        public void Update(Loan loan)
        {
            context.Set<Loan>().Update(loan);   
        }


        public int CalculateCreditScoreByAssetsAndLoan(decimal financialAssets, decimal loanAmount)
        {
            var ratio = loanAmount / financialAssets;
            if (financialAssets == 0 || ratio > 0.70m)
            {
                return 0;
            }
            else if (ratio < 0.50m)
            {
                return 20;
            }
            else
            {
                return 10;
            }
        }

        public int CalculateCreditScoreByCustomerJob(JobType jobType)
        {
            if (jobType == JobType.Employee)
            {
                return 20;
            }
            else if (jobType == JobType.SelfEmployed)
            {
                return 10;
            }
            else return 0;
        }

        public int CalculateCreditScoreByCustomerMonthlyIncome(decimal monthlyIncome)
        {
            if (monthlyIncome > 10000000)
            {
                return 20;
            }
            else if (5000000 <= monthlyIncome && monthlyIncome <= 10000000)
            {
                return 10;
            }
            else return 0;
        }

        public int CalculateUserCreditScoreByPaymentHistory(CustomerBackgroundDto customerPaymentBackground)
        {
            if (customerPaymentBackground.TotalLoansCount > 0
                && customerPaymentBackground.OverdueInstallmentCount == 0)
            {
                return 30;
            }
            else if (customerPaymentBackground.OverdueInstallmentCount > 0)
            {
                return customerPaymentBackground.OverdueInstallmentCount * -5;
            }
            else return 0;
        }

        public Loan? Find(int loanId)
        {
            return context.Set<Loan>().FirstOrDefault(_ => _.Id == loanId);
        }

        public void Register(Loan loan) => context.Set<Loan>().Add(loan);

        public bool IsExistById(int loanId)
        {
            return context.Set<Loan>().Any(_ => _.Id == loanId);
        }
    }
}
