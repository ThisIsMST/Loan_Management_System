﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace loanManagement.Services.LoanTemplates.Contracts.DTOs
{
    public class UpdateLoanTemplateDto
    {
        public decimal LoanAmount { get; set; }
        public int AnnualInterestRate { get; set; }
        public int DurationMonths { get; set; }
        public int InstallmentCount { get; set; }
    }
}
