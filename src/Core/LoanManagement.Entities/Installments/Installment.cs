using LoanManagement.Entities.Loans;

namespace LoanManagement.Entities.Installments
{
    public class Installment
    {
        public int Id { get; set; }
        public int LoanId { get; set; }
        public decimal InstallmentFine { get; set; } = 0;
        public decimal PaymentAmount { get; set; }
        public DateOnly? PaymentDate { get; set; }
        public DateOnly DueDate { get; set; }
        public InstallmentStatus InstallmentStatus { get; set; } = default;

    }

    public enum InstallmentStatus
    {
        Unpaid,
        Paid,
        Overdue,
    }
}
