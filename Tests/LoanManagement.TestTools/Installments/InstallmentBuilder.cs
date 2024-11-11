using LoanManagement.Entities.Installments;

namespace LoanManagement.TestTools.Installments
{
    public class InstallmentBuilder
    {
        private readonly Installment _installment;

        public InstallmentBuilder(int loanId)
        {
            _installment = new Installment
            {
                InstallmentStatus = InstallmentStatus.Unpaid,
                DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1)),
                InstallmentFine = 0,
                PaymentAmount = 1000000,
                LoanId = loanId
                
            };
        }

        public InstallmentBuilder WithPaymentDate(DateOnly date)
        {
            _installment.PaymentDate = date;
            return this;
        }
        public InstallmentBuilder WithInstallmentStatus(InstallmentStatus status)
        {
            _installment.InstallmentStatus = status;
            return this;
        }

        public InstallmentBuilder WithDueDate(DateOnly dueDate)
        {
            _installment.DueDate = dueDate;
            return this;
        }

        public InstallmentBuilder WithInstallmentFine(decimal fine)
        {
            _installment.InstallmentFine = fine;
            return this;
        }

        public InstallmentBuilder WithPaymentAmount(decimal amount)
        {
            _installment.PaymentAmount = amount;
            return this;
        }

        public Installment Build()
        {
            return _installment;
        }
    }
}

