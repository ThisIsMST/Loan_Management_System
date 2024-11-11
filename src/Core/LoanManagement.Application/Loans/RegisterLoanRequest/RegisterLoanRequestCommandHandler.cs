using loanManagement.Services.Loans.Contracts.Interfaces;
using loanManagement.Services.LoanTemplates.Contracts.Interfaces;
using loanManagement.Services.UnitOfWorks;
using loanManagement.Services.Users.Contracts.Interfaces;
using LoanManagement.Application.Loans.RegisterLoanRequest.Contracts;

namespace LoanManagement.Application.Loans.RegisterLoanRequest
{
    public class RegisterLoanRequestCommandHandler(
        LoanService loanService,
        LoanTemplateService loanTemplateService,
        UserService userService,
        UnitOfWork unitOfWork) : RegisterLoanRequestHandler
    {

        public void Handle(int customerId, int loanTemplateId)
        {
            unitOfWork.Begin();
            try
            {   var isCustomerVerified = userService.IsCustomerVerified(customerId);
                var AppliedLoan = loanTemplateService.GetLoanTemplateData(loanTemplateId);     
                var CustomerBackground = loanService.CheckCustomerBackground(customerId);
                var customerCreditScore = loanService.CalculateCustomerCreditScore(customerId,
                    CustomerBackground,
                    AppliedLoan);

                loanService.RegisterLoan(customerId,
                    AppliedLoan,
                    customerCreditScore,
                    CustomerBackground.CustomerPendingLoanRequestCount,
                    isCustomerVerified);
                userService.UpdateCustomerCreditScore(customerId, customerCreditScore);

                unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                throw ex;
            }


        }
    }
}
