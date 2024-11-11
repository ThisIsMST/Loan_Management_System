using loanManagement.Services.Installments.Contracts.Interfaces;
using LoanManagement.Entities.Installments;
using LoanManagement.Entities.Loans;
using LoanManagement.Persistence.EF.DataContext;

namespace LoanManagement.Persistence.EF.Installments
{
    public class EFInstallmentRepository(EFDataContext context) : InstallmentRepository
    {
        public Installment FindFirstUnpaidInstallment(int loanId)
        {
            return (from l in context.Set<Loan>()
                    join i in context.Set<Installment>()
                    on l.Id equals i.LoanId
                    where i.InstallmentStatus == InstallmentStatus.Unpaid
                    select i).OrderBy(_=>_.DueDate).First();
        }

        public void PayInstallment(Installment installment)
        {
            context.Set<Installment>().Update(installment);
        }

        public void ScheduleLoanInstallments(List<Installment> installments)
        {
            context.Set<Installment>().AddRange(installments);
        }
    }
}
