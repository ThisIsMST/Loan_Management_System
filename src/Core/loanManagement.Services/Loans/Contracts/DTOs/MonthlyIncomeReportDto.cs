namespace loanManagement.Services.Loans.Contracts.DTOs
{
        public class MonthlyIncomeReportDto
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public decimal TotalFines { get; set; }
            public decimal TotalInterest { get; set; }
            public decimal TotalIncome => TotalFines + TotalInterest;
        }
}
