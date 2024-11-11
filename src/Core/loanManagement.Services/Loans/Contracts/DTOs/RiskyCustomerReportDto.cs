using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace loanManagement.Services.Loans.Contracts.DTOs
{
    public class RiskyCustomerReportDto
    {
        public int UserId { get; set; }
        public required string FullName { get; set; }
        public int OverdueInstallmentsCount { get; set; }

    }
}
