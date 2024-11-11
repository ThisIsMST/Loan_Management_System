using System.ComponentModel.DataAnnotations.Schema;
using LoanManagement.Entities.Installments;
using LoanManagement.Entities.LoanTemplates;

namespace LoanManagement.Entities.Loans
{
    public class Loan
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal LoanAmount { get; set; }
        public int AnnualInterestRate { get; set; }
        [NotMapped]
        public decimal MonthlyInterestRate => AnnualInterestRate / 12;
        public int DurationMonths { get; set; }
        public DateOnly? StartDate { get; set; }
        public LoanStatus LoanStatus { get; set; }
        [NotMapped]
        public decimal MonthlyPayment => MonthlyInterestRate * LoanAmount;
        
    }
    public enum LoanStatus
    {
        Pending = 0,
        Approved,
        Rejected,
        Repaying,
        Overdue,
        Closed,
    }
}
