using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanManagement.Application.Loans.RegisterLoanRequest.Contracts
{
    public interface RegisterLoanRequestHandler
    {
        void Handle(int customerId, int loanTemplateId);
    }
}
