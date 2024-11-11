using LoanManagement.Entities.Loans;
using System.ComponentModel.DataAnnotations.Schema;

namespace loanManagement.Services.Loans.Contracts.DTOs
{
    public class RequestedLoanDto
    {
        public int LoanId { get; set; }
        public int CustomerId { get; set; }
        public decimal LoanAmount { get; set; }
        public int AnnualInterestRate { get; set; }

        public int DurationMonths { get; set; }
        [NotMapped]
        public decimal MonthlyInterestRate => AnnualInterestRate / 100m / 12;

        [NotMapped]
        public decimal MonthlyInterest => LoanAmount * MonthlyInterestRate;

        [NotMapped]
        public decimal MonthlyPayment => (LoanAmount / DurationMonths) + MonthlyInterest;

        /*
        [NotMapped]
        public decimal MonthlyPayment => (LoanAmount / DurationMonths) +
                                         (LoanAmount *
                                         (AnnualInterestRate / 100m / 12));
        */
    }
}

