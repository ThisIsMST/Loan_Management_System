using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoanManagement.Entities.Installments;

namespace loanManagement.Services.Installments.Contracts.Interfaces
{
    public interface InstallmentRepository
    {
        Installment FindFirstUnpaidInstallment(int loanId);
        void PayInstallment(Installment installment);
        void ScheduleLoanInstallments(List<Installment> installments);
    }
}
