using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoanManagement.Entities.Loans;

namespace loanManagement.Services.Loans.Contracts.DTOs
{
    public class ActiveAndOverDueLoansReportDto
    {
        public int LoanId { get; set; }
        public int UserId { get; set; }
        public decimal LoanAmount { get; set; }
        public LoanStatus CurrentStatus { get; set; }
        public decimal AmountPaid { get; set; }
        public int RemainingInstallments { get; set; }
    }
}
