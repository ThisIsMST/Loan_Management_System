using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanManagement.Application.Loans.ApplyLoanRequest.Contracts
{
    public interface ApproveLoanRequestHandler
    {
        void Handle(int customerId);
    }
}
