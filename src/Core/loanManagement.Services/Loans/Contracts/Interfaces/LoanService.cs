using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using loanManagement.Services.Loans.Contracts.DTOs;
using loanManagement.Services.LoanTemplates.Contracts.DTOs;

namespace loanManagement.Services.Loans.Contracts.Interfaces
{
    public interface LoanService
    {
        void ApproveLoan(int loanId);
        int CalculateCustomerCreditScore(int customerId, CustomerBackgroundDto customerPaymentBackGround, GetLoanTemplateDto appliedLoan);
        CustomerBackgroundDto CheckCustomerBackground(int customerId);
        RequestedLoanDto GetRequestedLoanByCustomerId(int customerId);
        int RegisterLoan(int customerId
            , GetLoanTemplateDto appliedLoan 
            , int customerCreditScore 
            , int PendingLoanRequest
            ,bool isCustomerVerified);
        void RejectLoan(int customerId);
        List<CustomerLoanDto> GetCustomerLoans(int customerId);
    }
}
