using FluentAssertions;
using loanManagement.Services.Installments;
using loanManagement.Services.Installments.Contracts.Interfaces;
using loanManagement.Services.Installments.Exceptions;
using loanManagement.Services.Loans.Contracts.DTOs;
using loanManagement.Services.Loans.Exceptions;
using LoanManagement.Entities.Installments;
using LoanManagement.Entities.Loans;
using LoanManagement.Persistence.EF.DataContext;
using LoanManagement.Persistence.EF.Installments;
using LoanManagement.Persistence.EF.Loans;
using LoanManagement.TestTools.Infrastructure.DataBaseConfig.Integration;
using LoanManagement.TestTools.Installments;
using LoanManagement.TestTools.Loans;

namespace LoanManagements.Service.Unit.Tests.Installments
{
    public class InstallmentServiceTest : BusinessIntegrationTest
    {
        private readonly InstallmentService _sut;

        public InstallmentServiceTest()
        {
            var repository = new EFInstallmentRepository(SetupContext);
            var loanRepository = new EFLoanRepository(SetupContext);
            var unitOfWork = new EFUnitOfWork(SetupContext);
            var installmentQuery = new EFInstallmentQuery(SetupContext);
            _sut = new InstallmentAppService(repository,
                loanRepository,
                unitOfWork,
                installmentQuery
                );
        }

        [Theory]
        [InlineData(100000 , 1 , 101250)]
        [InlineData(100000 , 2 , 51250)]
        public void ScheduleLoanInstallments_schedule_installment_properly(decimal fakeLoanAmount , int fakeDurationMonths , int fakeResult)
        {
            var dto = new RequestedLoanDto
            {
                LoanId = 1,
                LoanAmount = fakeLoanAmount,
                DurationMonths = fakeDurationMonths,
                AnnualInterestRate = 15,
            };
            _sut.ScheduleLoanInstallments(dto);

            var actual = ReadContext.Set<Installment>().FirstOrDefault(_ => _.LoanId == dto.LoanId);

            actual.Should().NotBeNull();
            actual!.DueDate.Should().Be(DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(1));
            actual.PaymentAmount.Should().Be(fakeResult);
        }

        [Theory]
        [InlineData(-1)]
        public void PayLoanInstallment_throw_exception_when_loan_is_not_exist(int fakeLoanId)
        {
            var actual = () => _sut.PayLoanInstallment(fakeLoanId);
            actual.Should().ThrowExactly<LoanNotFoundException>();
        }
        [Theory]
        [InlineData(1)]
        public void PayLoanInstallment_throw_exception_when_all_installments_paid_successfully(int fakeCustomerId)
        {
            var loan = new LoanBuilder(fakeCustomerId).Build();
            Save(loan);
            var installment = new InstallmentBuilder(loan.Id)
                .WithInstallmentStatus(InstallmentStatus.Paid).Build();
            Save(installment);
            var actual =()=> _sut.PayLoanInstallment(loan.Id);
            actual.Should().ThrowExactly<AllInstallmentsPaidException>();
        }
        [Theory]
        [InlineData(1)]
        public void PayLoanInstallment_close_loan_when_last_installment_paid(int fakeCustomerId)
        {
            var loan = new LoanBuilder(fakeCustomerId).Build();
            Save(loan);
            var installment = new InstallmentBuilder(loan.Id)
                .WithInstallmentStatus(InstallmentStatus.Unpaid).Build();
            Save(installment);

            _sut.PayLoanInstallment(loan.Id);

            var actualInstallment = ReadContext.Set<Installment>().SingleOrDefault(_ => _.Id == installment.Id);
            var actualLoan = ReadContext.Set<Loan>().SingleOrDefault(_=>_.Id == loan.Id);

            actualInstallment.Should().NotBeNull();
            actualInstallment!.InstallmentStatus.Should().Be(InstallmentStatus.Paid);
            actualLoan.Should().NotBeNull();
            actualLoan!.LoanStatus.Should().Be(LoanStatus.Closed);
        }

        [Theory]
        [InlineData(1)]
        public void PayLoanInstallment_change_loan_status_to_repaying_when_first_payment_done(int fakeCustomerId)
        {
            var loan = new LoanBuilder(fakeCustomerId).WithLoanStatus(LoanStatus.Approved).Build();
            Save(loan);
            var installment1 = new InstallmentBuilder(loan.Id)
                .WithInstallmentStatus(InstallmentStatus.Unpaid).WithDueDate(DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1))).Build();
            Save(installment1);
            var installment2 = new InstallmentBuilder(loan.Id)
                .WithInstallmentStatus(InstallmentStatus.Unpaid).WithDueDate(DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(2))).Build();
            Save(installment2);

            _sut.PayLoanInstallment(loan.Id);
            var actualLoan = ReadContext.Set<Loan>().SingleOrDefault(_=>_.Id == loan.Id);
            var actualInstallment1 = ReadContext.Set<Installment>().SingleOrDefault(_=>_.Id == installment1.Id);
            var actualInstallment2 = ReadContext.Set<Installment>().SingleOrDefault(_ => _.Id == installment2.Id);

            actualLoan.Should().NotBeNull();
            actualLoan!.LoanStatus.Should().Be(LoanStatus.Repaying);
            actualInstallment1.Should().NotBeNull();
            actualInstallment1!.InstallmentStatus.Should().Be(InstallmentStatus.Paid);
            actualInstallment2.Should().NotBeNull();
            actualInstallment2!.InstallmentStatus.Should().Be(InstallmentStatus.Unpaid);

        }
        [Theory]
        [InlineData(1)]
        public void PayLoanInstallment_keep_loan_status_to_repaying_when_loan_has_unpaid_installment(int fakeCustomerId)
        {
            var loan = new LoanBuilder(fakeCustomerId).WithLoanStatus(LoanStatus.Repaying).Build();
            Save(loan);
            var installment1 = new InstallmentBuilder(loan.Id)
                .WithInstallmentStatus(InstallmentStatus.Unpaid).WithDueDate(DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1))).Build();
            Save(installment1);
            var installment2 = new InstallmentBuilder(loan.Id)
                .WithInstallmentStatus(InstallmentStatus.Unpaid).WithDueDate(DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(2))).Build();
            Save(installment2);

            _sut.PayLoanInstallment(loan.Id);
            var actualLoan = ReadContext.Set<Loan>().SingleOrDefault(_ => _.Id == loan.Id);
            var actualInstallment1 = ReadContext.Set<Installment>().SingleOrDefault(_ => _.Id == installment1.Id);
            var actualInstallment2 = ReadContext.Set<Installment>().SingleOrDefault(_ => _.Id == installment2.Id);

            actualLoan.Should().NotBeNull();
            actualLoan!.LoanStatus.Should().Be(LoanStatus.Repaying);
            actualInstallment1.Should().NotBeNull();
            actualInstallment1!.InstallmentStatus.Should().Be(InstallmentStatus.Paid);
            actualInstallment2.Should().NotBeNull();
            actualInstallment2!.InstallmentStatus.Should().Be(InstallmentStatus.Unpaid);

        }
        [Theory]
        [InlineData(1)]
        public void PayLoanInstallment_change_loan_status_to_overdue_when_payment_get_late(int FakeCustomerId)
        {
            var loan = new LoanBuilder(FakeCustomerId).WithLoanStatus(LoanStatus.Repaying).Build();
            Save(loan);
            var installment1 = new InstallmentBuilder(loan.Id)
                .WithInstallmentStatus(InstallmentStatus.Unpaid)
                .WithDueDate(DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1).AddDays(-5))).Build();
            Save(installment1);
            var installment2 = new InstallmentBuilder(loan.Id)
                .WithInstallmentStatus(InstallmentStatus.Unpaid).WithDueDate(DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(2))).Build();
            Save(installment2);

            _sut.PayLoanInstallment(loan.Id);
            var actualLoan = ReadContext.Set<Loan>().SingleOrDefault(_ => _.Id == loan.Id);
            var actualInstallment1 = ReadContext.Set<Installment>().SingleOrDefault(_ => _.Id == installment1.Id);
            var actualInstallment2 = ReadContext.Set<Installment>().SingleOrDefault(_ => _.Id == installment2.Id);

            actualLoan.Should().NotBeNull();
            actualLoan!.LoanStatus.Should().Be(LoanStatus.Overdue);
            actualInstallment1.Should().NotBeNull();
            actualInstallment1!.InstallmentStatus.Should().Be(InstallmentStatus.Overdue);
            actualInstallment2.Should().NotBeNull();
            actualInstallment2!.InstallmentStatus.Should().Be(InstallmentStatus.Unpaid);

        }
    }
}
