using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoanManagement.Entities.Loans;

namespace loanManagement.Services.Loans.Contracts.DTOs
{
    public class CustomerLoanDto
    {
        public int Id { get; set; }
        public decimal LoanAmount { get; set; }
        public LoanStatus LoanStatus { get; set; }
        public int AnnualInterestRate { get; set; }
        public int DurationMonths { get; set; }
        public DateOnly StartDate { get; set; }
    }
}
