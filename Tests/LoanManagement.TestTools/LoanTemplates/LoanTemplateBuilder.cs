using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoanManagement.Entities.LoanTemplates;

namespace LoanManagement.TestTools.LoanTemplates
{
    public class LoanTemplateBuilder
    {
        private readonly LoanTemplate _loanTemplate;

        public LoanTemplateBuilder()
        {
            _loanTemplate = new LoanTemplate
            {
                LoanAmount = 100000,
                AnnualInterestRate = 15,
                DurationMonths = 12
            };
        }

        public LoanTemplateBuilder WithLoanAmount(decimal loanAmount)
        {
            _loanTemplate.LoanAmount = loanAmount;
            return this;
        }

        public LoanTemplateBuilder WithAnnualInterestRate(int annualInterestRate)
        {
            _loanTemplate.AnnualInterestRate = annualInterestRate;
            return this;
        }

        public LoanTemplateBuilder WithDurationMonths(int durationMonths)
        {
            _loanTemplate.DurationMonths = durationMonths;
            return this;
        }

        public LoanTemplate Build()
        {
            return _loanTemplate;
        }
    }
}
