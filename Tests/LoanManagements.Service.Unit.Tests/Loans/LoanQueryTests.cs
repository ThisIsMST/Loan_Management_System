using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using loanManagement.Services.Loans.Contracts.DTOs;
using loanManagement.Services.Loans.Contracts.Interfaces;
using LoanManagement.Entities.Installments;
using LoanManagement.Entities.Loans;
using LoanManagement.Persistence.EF.Loans;
using LoanManagement.TestTools.Infrastructure.DataBaseConfig.Integration;
using LoanManagement.TestTools.Installments;
using LoanManagement.TestTools.Loans;
using LoanManagement.TestTools.Roles;
using LoanManagement.TestTools.Users;

namespace LoanManagements.Service.Unit.Tests.Loans
{
    public class LoanQueryTests : BusinessIntegrationTest
    {
        private readonly LoanQuery _sut;

        public LoanQueryTests()
        {
            _sut = new EFLoanQuery(SetupContext);
        }

        [Fact]
        public void GetActiveAndDelayedLoansReport_get_active_and_overdue_loans_report_properly()
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var user = new CustomerBuilder(role.Id).Build();
            Save(user);
            var activeLoan = new LoanBuilder(user.Id)
                .WithLoanStatus(LoanStatus.Repaying)
                .Build();
            Save(activeLoan);
            var overdueLoan = new LoanBuilder(user.Id)
                .WithLoanStatus(LoanStatus.Overdue)
                .Build();
            Save(overdueLoan);

            var result = _sut.GetActiveAndDelayedLoansReport();

            var expectedLoans = new List<ActiveAndOverDueLoansReportDto>
            {
                new ActiveAndOverDueLoansReportDto
                {
                    LoanId = activeLoan.Id,
                    UserId = user.Id,
                    LoanAmount = activeLoan.LoanAmount,
                    CurrentStatus = activeLoan.LoanStatus
                },
                new ActiveAndOverDueLoansReportDto
                {
                    LoanId = overdueLoan.Id,
                    UserId = user.Id,
                    LoanAmount = overdueLoan.LoanAmount,
                    CurrentStatus = overdueLoan.LoanStatus
                },
             };
            result.Should().BeEquivalentTo(expectedLoans);
        }
        [Fact]
        public void GetClosedLoansReport_get_closed_loans_report_properly()
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var user = new CustomerBuilder(role.Id).Build();
            Save(user);

            var closedLoan = new LoanBuilder(user.Id)
                .WithLoanStatus(LoanStatus.Closed)
                .WithDurationMonths(2)
                .Build();
            Save(closedLoan);

            var installment1 = new InstallmentBuilder(closedLoan.Id)
                .WithInstallmentStatus(InstallmentStatus.Paid)
                .WithInstallmentFine(100)
                .Build();
            var installment2 = new InstallmentBuilder(closedLoan.Id)
                .WithInstallmentStatus(InstallmentStatus.Overdue)
                .WithInstallmentFine(150)
                .Build();
            Save(installment1);
            Save(installment2);

            var result = _sut.GetClosedLoansReport();

            result.Should().ContainEquivalentOf(new ClosedLoanReportDto
            {
                LoanId = closedLoan.Id,
                UserId = user.Id,
                LoanAmount = closedLoan.LoanAmount,
                TotalInstallmentsToPaid = closedLoan.DurationMonths,
                TotalFinePaid = 250
            });
        }
        [Fact]
        public void GetRiskyCustomersReports_get_risky_customers_report_properly()
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var user = new CustomerBuilder(role.Id).Build();
            Save(user);
            var loan = new LoanBuilder(user.Id)
                .WithLoanStatus(LoanStatus.Overdue)
                .WithDurationMonths(3)
                .Build();
            Save(loan);

            var overdueInstallment1 = new InstallmentBuilder(loan.Id)
                .WithInstallmentStatus(InstallmentStatus.Overdue)
                .Build();
            var overdueInstallment2 = new InstallmentBuilder(loan.Id)
                .WithInstallmentStatus(InstallmentStatus.Overdue)
                .Build();
            var overdueInstallment3 = new InstallmentBuilder(loan.Id)
                .WithInstallmentStatus(InstallmentStatus.Overdue)
                .Build();
            Save(overdueInstallment1);
            Save(overdueInstallment2);
            Save(overdueInstallment3);

            var result = _sut.GetRiskyCustomersReports();
            result.Should().BeEquivalentTo(new List<RiskyCustomerReportDto> {
            new RiskyCustomerReportDto
            {
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                OverdueInstallmentsCount = 3
            } }
            );

        }
        [Fact]
        public void GetMonthlyIncomeReport_get_monthly_income_properly()
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);

            var user = new CustomerBuilder(role.Id).Build();
            Save(user);

            var currentTime = DateOnly.FromDateTime(DateTime.UtcNow);
            var loan = new LoanBuilder(user.Id)
                .WithLoanStatus(LoanStatus.Repaying)
                .Build();
            Save(loan);
            var overdueDate1 = currentTime.AddMonths(-1).AddDays(-1);
            var installment1 = new InstallmentBuilder(loan.Id)
                  .WithPaymentDate(overdueDate1)
                  .WithInstallmentFine(10)
                  .WithInstallmentStatus(InstallmentStatus.Overdue)
                  .Build();
            Save(installment1);
            var overdueDate2 = overdueDate1.AddMonths(-1);
            var installment2 = new InstallmentBuilder(loan.Id)
                .WithPaymentDate(overdueDate2)
                .WithInstallmentFine(15)
                .Build();
            Save(installment2);
            var overdueDate3 = overdueDate2.AddMonths(-1);
            var installment3 = new InstallmentBuilder(loan.Id)
                .WithPaymentDate(overdueDate3)
                .WithInstallmentFine(20)
                .Build();
            Save(installment3);


            var result = _sut.GetMonthlyIncomeReport();

            var expectedReports = new List<MonthlyIncomeReportDto>
    {
        new MonthlyIncomeReportDto
        {
            Month = overdueDate1.Month,
            Year = overdueDate1.Year,
            TotalFines = 10,
            TotalInterest = loan.MonthlyPayment * 0.2m,
        },
        new MonthlyIncomeReportDto
        {
            Month = overdueDate2.Month,
            Year = overdueDate2.Year,
            TotalFines = 15,
            TotalInterest =  loan.MonthlyPayment * 0.02m
        },
        new MonthlyIncomeReportDto
        {
            Month = overdueDate3.Month,
            Year = overdueDate3.Year,
            TotalFines = 20,
            TotalInterest =  loan.MonthlyPayment * 0.02m
        },
        new MonthlyIncomeReportDto
        {
            Month = currentTime.Month,
            Year = currentTime.Year,
            TotalFines = 00,
            TotalInterest =  00
        }
    };

            result.Should().BeEquivalentTo(expectedReports);
        }
        [Fact]
        public void CheckCustomerBackground_get_customer_background_properly()
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var user = new CustomerBuilder(role.Id).Build();
            Save(user);
            var loan1 = new LoanBuilder(user.Id)
                .WithLoanStatus(LoanStatus.Repaying)
                .Build();
            var loan2 = new LoanBuilder(user.Id)
                .WithLoanStatus(LoanStatus.Pending)
                .Build();
            Save(loan1, loan2);
            var overdueInstallment = new InstallmentBuilder(loan1.Id)
                .WithInstallmentStatus(InstallmentStatus.Overdue)
                .Build();
            Save(overdueInstallment);

            var result = _sut.CheckCustomerBackground(user.Id);

            result.Should().BeEquivalentTo(new CustomerBackgroundDto
            {
                OverdueInstallmentCount = 1,
                TotalLoansCount = 2,
                CustomerPendingLoanRequestCount = 1
            });
        }

        [Fact]
        public void GetRequestedLoanByCustomerId_get_requested_loan_by_customer_id()
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var user = new CustomerBuilder(role.Id).Build();
            Save(user);
            var loan = new LoanBuilder(user.Id)
                .WithLoanStatus(LoanStatus.Pending)
                .WithAnnualInterestRate(20)
                .WithDurationMonths(24)
                .WithLoanAmount(100000)
                .Build();
            Save(loan);

            var result = _sut.GetRequestedLoanByCustomerId(user.Id);

            result.Should().BeEquivalentTo(new RequestedLoanDto
            {
                CustomerId = user.Id,
                LoanId = loan.Id,
                DurationMonths = 24,
                AnnualInterestRate = 20,
                LoanAmount = 100000
            });
        }

        [Fact]
        public void GetCustomerLoans_get_customer_loans_properly()
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id).Build();
            Save(customer);
            var loan = new LoanBuilder(customer.Id).WithStartDate(DateOnly.FromDateTime(DateTime.UtcNow)).Build();
            Save(loan);

            var actual = _sut.GetCustomerLoans(customer.Id);

            actual.Should().BeEquivalentTo(new List<CustomerLoanDto>
            {
                new CustomerLoanDto
                {
                    Id = loan.Id,
                    DurationMonths = loan.DurationMonths,
                    StartDate = (DateOnly)loan.StartDate,
                    AnnualInterestRate = loan.AnnualInterestRate,
                    LoanStatus = loan.LoanStatus,
                    LoanAmount = loan.LoanAmount
                }
             });

        }
    }
}