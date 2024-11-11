using loanManagement.Services.Installments.Contracts.Interfaces;
using LoanManagement.Entities.Installments;
using LoanManagement.Persistence.EF.DataContext;

namespace LoanManagement.Persistence.EF.Installments
{
    public class EFInstallmentQuery(EFDataContext context) : InstallmentQuery
    {
        public int GetRemainingInstallmentByLoanId(int loanId)
        {
            return (
                from i in context.Set<Installment>()
                where i.LoanId == loanId
                && i.InstallmentStatus == InstallmentStatus.Unpaid
                select i
                ).Count();
        }
    }
}
