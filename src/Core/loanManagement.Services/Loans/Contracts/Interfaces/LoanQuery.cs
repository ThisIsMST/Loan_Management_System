using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using loanManagement.Services.Loans.Contracts.DTOs;

namespace loanManagement.Services.Loans.Contracts.Interfaces
{
    public interface LoanQuery
    {
        CustomerBackgroundDto CheckCustomerBackground(int customerId);
        RequestedLoanDto GetRequestedLoanByCustomerId(int customerId);
        List<ActiveAndOverDueLoansReportDto> GetActiveAndDelayedLoansReport();
        List<ClosedLoanReportDto> GetClosedLoansReport();
        List<MonthlyIncomeReportDto> GetMonthlyIncomeReport();
        
        List<RiskyCustomerReportDto> GetRiskyCustomersReports();
        List<CustomerLoanDto> GetCustomerLoans(int customerId);
    }
}
