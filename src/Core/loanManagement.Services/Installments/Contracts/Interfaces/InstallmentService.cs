using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using loanManagement.Services.Loans.Contracts.DTOs;

namespace loanManagement.Services.Installments.Contracts.Interfaces
{
    public interface InstallmentService
    {
        void PayLoanInstallment(int loanId);
        void ScheduleLoanInstallments(RequestedLoanDto requestedLoan);
    }
}
