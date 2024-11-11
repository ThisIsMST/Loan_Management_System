using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace loanManagement.Services.Installments.Contracts.Interfaces
{
    public interface InstallmentQuery
    {
        int GetRemainingInstallmentByLoanId(int loanId);
    }
}
