using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoanManagement.Entities.Loans;

namespace LoanManagement.Entities.LoanTemplates
{
    public class LoanTemplate
    {
        public int Id { get; set; }
        public decimal LoanAmount { get; set; }
        public int AnnualInterestRate { get; set; }
        public int DurationMonths { get; set; }
        [NotMapped]
        public int InstallmentCount => DurationMonths; /*
                                                   i know that's the same thing in this business but i thought it'll be better in case we wanna develop it
                                                   for instance we have a loan with 10 months duration and 2 installment count */
    }
}
