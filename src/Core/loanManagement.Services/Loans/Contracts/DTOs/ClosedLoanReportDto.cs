using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace loanManagement.Services.Loans.Contracts.DTOs
{
    public class ClosedLoanReportDto
    {
        public int LoanId { get; set; }
        public int UserId { get; set; }
        public decimal LoanAmount { get; set; }
        public int TotalInstallmentsToPaid { get; set; }
        public decimal TotalFinePaid { get; set; }
    }
}
