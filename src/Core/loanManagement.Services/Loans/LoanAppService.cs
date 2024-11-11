using loanManagement.Services.Loans.Contracts.DTOs;
using loanManagement.Services.Loans.Contracts.Interfaces;
using loanManagement.Services.Loans.Exceptions;
using loanManagement.Services.LoanTemplates.Contracts.DTOs;
using loanManagement.Services.UnitOfWorks;
using loanManagement.Services.Users.Contracts.Interfaces;
using loanManagement.Services.Users.Exceptions;
using LoanManagement.Entities.Loans;

namespace loanManagement.Services.Loans
{
    public class LoanAppService(
        LoanRepository repository,
        UserRepository userRepository,
        LoanQuery loanQuery,
        UserQuery userQuery,
        UnitOfWork unitOfWork)
         : LoanService
    {
        public void ApproveLoan(int loanId)
        {
            var loan = repository.Find(loanId);
            if (loan == null)
            {
                throw new LoanNotFoundException();
            }
            loan.StartDate = DateOnly.FromDateTime(DateTime.UtcNow);
            loan.LoanStatus = LoanStatus.Approved;
            repository.Update(loan);
            unitOfWork.Save();
        }

        public int CalculateCustomerCreditScore(int customerId, CustomerBackgroundDto customerPaymentBackground, GetLoanTemplateDto appliedLoan)
        {
            var isCustomerExistById = userRepository.IsExistById(customerId);
            if (!isCustomerExistById)
            {
                throw new CustomerNotFoundException();
            }
            int customerCreditScore = 0;

            var customerFinancial = userQuery.GetCustomerFinancialData(customerId);

            customerCreditScore += repository.
                CalculateCreditScoreByAssetsAndLoan(customerFinancial.FinancialAssets, appliedLoan.LoanAmount);

            customerCreditScore += repository.
                CalculateCreditScoreByCustomerJob(customerFinancial.JobType);

            customerCreditScore += repository.
                CalculateCreditScoreByCustomerMonthlyIncome(customerFinancial.MonthlyIncome);

            customerCreditScore += repository.
                CalculateUserCreditScoreByPaymentHistory(customerPaymentBackground);

            return customerCreditScore;
        }

        public CustomerBackgroundDto CheckCustomerBackground(int customerId)
        {
            var isCustomerExistById = userRepository.IsExistById(customerId);
            if (!isCustomerExistById)
            {
                throw new CustomerNotFoundException();
            }
            return loanQuery.CheckCustomerBackground(customerId);
        }

        public List<CustomerLoanDto> GetCustomerLoans(int customerId)
        {
            var loans = loanQuery.GetCustomerLoans(customerId);
            if (loans.Count == 0)
            {
                throw new CustomerDoesNotHaveLoan();
            }
            return loans;
        }

        public RequestedLoanDto GetRequestedLoanByCustomerId(int customerId)
        {
            var isCustomerExistById = userRepository.IsExistById(customerId);
            if (!isCustomerExistById)
            {
                throw new CustomerNotFoundException();
            }
            return loanQuery.GetRequestedLoanByCustomerId(customerId);
        }

        public int RegisterLoan(int customerId
            , GetLoanTemplateDto appliedLoan
            , int customerCreditScore
            , int PendingLoanRequest 
            , bool isCustomerVerified)
        {
            var isCustomerExistById = userRepository.IsExistById(customerId);
            if (!isCustomerExistById)
            {
                throw new CustomerNotFoundException();
            }
            if (!isCustomerVerified)
            {
                throw new CustomerNotVerifiedException();
            }
            if (PendingLoanRequest != 0)
            {
                throw new CustomerHaveLoanToApproveException();
            }
            if (customerCreditScore < 60)
            {
                throw new CustomerIsNotQualifiedForApplyingException();
            }
            else
            {
                var loan = new Loan
                {
                    UserId = customerId,
                    LoanStatus = LoanStatus.Pending,
                    DurationMonths = appliedLoan.DurationMonths,
                    AnnualInterestRate = appliedLoan.DurationMonths < 12 ? 15 : 20,
                    LoanAmount = appliedLoan.LoanAmount,
                };
                repository.Register(loan);
                unitOfWork.Save();
                return loan.Id;
            }
        }

        public void RejectLoan(int customerId)
        {
            var isCustomerExistById = userRepository.IsExistById(customerId);
            if (!isCustomerExistById)
            {
                throw new CustomerNotFoundException();
            }
            var requestedLoan = loanQuery.GetRequestedLoanByCustomerId(customerId);
            var loan = repository.Find(requestedLoan.LoanId);
            if (loan == null)
            {
                throw new LoanNotFoundException();
            }
            loan.LoanStatus = LoanStatus.Rejected;
            repository.Update(loan);
            unitOfWork.Save();

        }
    }
}


