using loanManagement.Services.Installments.Contracts.Interfaces;
using loanManagement.Services.Loans.Contracts.Interfaces;
using loanManagement.Services.UnitOfWorks;
using loanManagement.Services.Users.Contracts.Interfaces;
using LoanManagement.Application.Loans.ApplyLoanRequest.Contracts;

namespace LoanManagement.Application.Loans.ApplyLoanRequest
{
    public class ApproveLoanRequestCommandHandler(
        LoanService loanService,
        UserService userService,
        InstallmentService installmentService , 
        UnitOfWork unitOfWork
        ) : ApproveLoanRequestHandler
    {
        public void Handle(int customerId)
        {
            unitOfWork.Begin();
            try
            {
                var requestedLoan = loanService.GetRequestedLoanByCustomerId(customerId);
                loanService.ApproveLoan(requestedLoan.LoanId);
                userService.AddLoanToCustomerAssets(requestedLoan.CustomerId, requestedLoan.LoanAmount);
                installmentService.ScheduleLoanInstallments(requestedLoan);
                unitOfWork.Commit();
            }   
            catch(Exception ex) 
            {
                unitOfWork.Rollback();
                throw ex;
            }
        }
    }
}
