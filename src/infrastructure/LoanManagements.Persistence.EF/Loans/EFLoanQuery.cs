using loanManagement.Services.Loans.Contracts.DTOs;
using loanManagement.Services.Loans.Contracts.Interfaces;
using loanManagement.Services.Loans.Exceptions;
using LoanManagement.Entities.Installments;
using LoanManagement.Entities.Loans;
using LoanManagement.Entities.Users;
using LoanManagement.Persistence.EF.DataContext;


namespace LoanManagement.Persistence.EF.Loans
{
    public class EFLoanQuery(EFDataContext context) : LoanQuery
    {
        public CustomerBackgroundDto CheckCustomerBackground(int customerId)
        {
            var CustomerLoansHistory = (from l in context.Set<Loan>()
                                        where l.UserId == customerId
                                        select new Loan
                                        {
                                            Id = l.Id,

                                        }).ToList();

            var CustomerPaymentHistory = (from l in CustomerLoansHistory
                                          join i in context.Set<Installment>()
                                          on l.Id equals i.LoanId
                                          where i.InstallmentStatus is InstallmentStatus.Overdue
                                          select new
                                          {
                                              installmentStatus = i.InstallmentStatus,
                                          }).Count();
            var customerPendingLoanRequestCount = (from l in context.Set<Loan>()
                                                   where l.LoanStatus == LoanStatus.Pending
                                                   && l.UserId == customerId
                                                   select new
                                                   {
                                                       PendingLoanRequest = l.LoanStatus,
                                                   }
                                                   ).Count();

            return new CustomerBackgroundDto
            {
                OverdueInstallmentCount = CustomerPaymentHistory,
                TotalLoansCount = CustomerLoansHistory.Count,
                CustomerPendingLoanRequestCount = customerPendingLoanRequestCount,
            };

        }
        public RequestedLoanDto GetRequestedLoanByCustomerId(int customerId)
        {
            return (
                from c in context.Set<User>()
                join l in context.Set<Loan>()
                on c.Id equals l.UserId
                where l.LoanStatus == LoanStatus.Pending
                select new RequestedLoanDto
                {
                    CustomerId = customerId,
                    LoanId = l.Id,
                    AnnualInterestRate = l.AnnualInterestRate,
                    DurationMonths = l.DurationMonths,
                    LoanAmount = l.LoanAmount,
                }
                ).Single();
        }
        public List<ActiveAndOverDueLoansReportDto> GetActiveAndDelayedLoansReport()
        {
            return (from l in context.Set<Loan>()
                    where l.LoanStatus == LoanStatus.Repaying ||
                    l.LoanStatus == LoanStatus.Overdue
                    select new ActiveAndOverDueLoansReportDto
                    {
                        LoanId = l.Id,
                        UserId = l.UserId,
                        LoanAmount = l.LoanAmount,
                        CurrentStatus = l.LoanStatus,
                        AmountPaid = (from i in context.Set<Installment>()
                                      where i.LoanId == l.Id &&
                                      i.InstallmentStatus == InstallmentStatus.Paid ||
                                      i.InstallmentStatus == InstallmentStatus.Overdue
                                      select i.PaymentAmount).Sum(),
                        RemainingInstallments = (from installment in context.Set<Installment>()
                                                 where installment.LoanId == l.Id &&
                                                 installment.InstallmentStatus == InstallmentStatus.Unpaid
                                                 select installment).Count()
                    }).ToList();

        }
        public List<ClosedLoanReportDto> GetClosedLoansReport()
        {
            return (from l in context.Set<Loan>()
                    where l.LoanStatus == LoanStatus.Closed
                    select new ClosedLoanReportDto
                    {
                        LoanId = l.Id,
                        UserId = l.UserId,
                        LoanAmount = l.LoanAmount,
                        TotalInstallmentsToPaid = l.DurationMonths,
                        TotalFinePaid = (from installment in context.Set<Installment>()
                                         where installment.LoanId == l.Id &&
                                         (installment.InstallmentStatus == InstallmentStatus.Paid ||
                                         installment.InstallmentStatus == InstallmentStatus.Overdue)
                                         select installment.InstallmentFine).Sum()
                    }).ToList();
        }
        public List<MonthlyIncomeReportDto> GetMonthlyIncomeReport()
        {
            var firstPayment = context.Set<Installment>().
                OrderBy(_ => _.PaymentDate).
                Select(_ => _.PaymentDate).
                FirstOrDefault();

            if (firstPayment == null)
            {
                throw new NoPaymentLogsFoundException();
            }

            var MonthlyReports = new List<MonthlyIncomeReportDto>();

            var startOfTempMonth = new DateOnly(firstPayment.Value.Year, firstPayment.Value.Month, 1);
            var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);

            while (startOfTempMonth < currentDate)
            {
                var endOfTempMonth = startOfTempMonth.AddMonths(1);

                var totalFines = (from i in context.Set<Installment>()
                                  where i.PaymentDate >= startOfTempMonth &&
                                  i.PaymentDate < endOfTempMonth
                                  select i).ToList().Sum(_=>_.InstallmentFine);


                var totalInterests = (from i in context.Set<Installment>()
                                      join l in context.Set<Loan>() on i.LoanId equals l.Id
                                      where i.PaymentDate >= startOfTempMonth
                                      && i.PaymentDate < endOfTempMonth
                                      select l).ToList().Sum(_=>_.MonthlyPayment * _.MonthlyInterestRate);
                                     


                MonthlyReports.Add(new MonthlyIncomeReportDto
                {
                    Month = startOfTempMonth.Month,
                    Year = startOfTempMonth.Year,
                    TotalFines = totalFines,
                    TotalInterest = totalInterests,

                });

                startOfTempMonth = endOfTempMonth;

            }
            return MonthlyReports;

        }
        public List<RiskyCustomerReportDto> GetRiskyCustomersReports()
        {
            return (
                from c in context.Set<User>()
                join l in context.Set<Loan>()
                on c.Id equals l.UserId
                where l.LoanStatus == LoanStatus.Overdue || l.LoanStatus == LoanStatus.Closed
                let OverdueInstallmentCount = (from i in context.Set<Installment>()
                                               where i.LoanId == l.Id &&
                                               i.InstallmentStatus == InstallmentStatus.Overdue
                                               select i).Count()
                where OverdueInstallmentCount> 2
                select new RiskyCustomerReportDto
                {
                    FullName = c.FirstName + " " + c.LastName,
                    UserId = c.Id,
                    OverdueInstallmentsCount = OverdueInstallmentCount
                }
                ).ToList();
        }

        public List<CustomerLoanDto> GetCustomerLoans(int customerId)
        {
            return (
                from c in context.Set<User>()
                where c.Id == customerId
                join l in context.Set<Loan>()
                on c.Id equals l.UserId
                select new CustomerLoanDto
                {
                    Id = l.Id,
                    AnnualInterestRate = l.AnnualInterestRate,
                    DurationMonths = l.DurationMonths,
                    LoanAmount = l.LoanAmount,
                    StartDate = (DateOnly)l.StartDate,
                    LoanStatus = l.LoanStatus,
                }
                ).ToList();
        }
    }
}
