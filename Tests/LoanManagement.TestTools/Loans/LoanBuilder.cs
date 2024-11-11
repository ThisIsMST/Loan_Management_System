using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoanManagement.Entities.Loans;

namespace LoanManagement.TestTools.Loans
{
    public class LoanBuilder
    {
        private readonly Loan _loan;

        public LoanBuilder(int CustomerId)
        {
            _loan = new Loan
            {
                UserId = CustomerId,
                LoanAmount = 10000000,
                AnnualInterestRate = 10,
                DurationMonths = 10,
                LoanStatus = LoanStatus.Pending ,
            };
        }

        public LoanBuilder WithLoanAmount(decimal loanAmount)
        {
            _loan.LoanAmount = loanAmount;
            return this;
        }

        public LoanBuilder WithAnnualInterestRate(int annualInterestRate)
        {
            _loan.AnnualInterestRate = annualInterestRate;
            return this;
        }

        public LoanBuilder WithDurationMonths(int durationMonths)
        {
            _loan.DurationMonths = durationMonths;
            return this;
        }

        public LoanBuilder WithStartDate(DateOnly? startDate)
        {
            _loan.StartDate = startDate;
            return this;
        }

        public LoanBuilder WithLoanStatus(LoanStatus loanStatus)
        {
            _loan.LoanStatus = loanStatus;
            return this;
        }

        public Loan Build() => _loan;
    }
}

