using FluentAssertions;
using loanManagement.Services.Loans;
using loanManagement.Services.Loans.Contracts.DTOs;
using loanManagement.Services.Loans.Contracts.Interfaces;
using loanManagement.Services.Loans.Exceptions;
using loanManagement.Services.LoanTemplates.Contracts.DTOs;
using loanManagement.Services.Users.Exceptions;
using LoanManagement.Entities.Installments;
using LoanManagement.Entities.Loans;
using LoanManagement.Entities.Users;
using LoanManagement.Persistence.EF.DataContext;
using LoanManagement.Persistence.EF.Loans;
using LoanManagement.Persistence.EF.Users;
using LoanManagement.TestTools.Infrastructure.DataBaseConfig.Integration;
using LoanManagement.TestTools.Installments;
using LoanManagement.TestTools.Loans;
using LoanManagement.TestTools.Roles;
using LoanManagement.TestTools.Users;

namespace LoanManagements.Service.Unit.Tests.Loans
{
    public class LoanServiceTests : BusinessIntegrationTest
    {
        private readonly LoanService _sut;

        public LoanServiceTests()
        {
            var repository = new EFLoanRepository(SetupContext);
            var loneQuery = new EFLoanQuery(SetupContext);
            var userQuery = new EFUserQuery(SetupContext);
            var unitOfWork = new EFUnitOfWork(SetupContext);
            var userRepository = new EFUserRepository(SetupContext);

            _sut = new LoanAppService(
                repository,
                userRepository,
                loneQuery,
                userQuery,
                unitOfWork
                );
        }
        [Theory]
        [InlineData(1)]
        public void ApproveLoan_approve_loan_properly(int userId)
        {

            var loan = new LoanBuilder(userId).Build();
            Save(loan);
            _sut.ApproveLoan(loan.Id);

            var actual = ReadContext.Set<Loan>().Single(_ => _.Id == loan.Id);

            actual.Should().NotBeNull();
            actual.LoanStatus.Should().Be(LoanStatus.Approved);
            actual.StartDate.Should().Be(DateOnly.FromDateTime(DateTime.UtcNow));

        }

        [Fact]
        public void CalculateCustomerCreditScore_calculate_properly()
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id).
                WithJobType(JobType.Employee) //+20
                .WithMonthlyIncome(9000000) //+10
                .WithFinancialAssets(100000000) //100mil
                .Build();
            Save(customer);
            var customerBackground = new CustomerBackgroundDto
            {
                CustomerPendingLoanRequestCount = 1,
                OverdueInstallmentCount = 3, //-15
                TotalLoansCount = 1,
            };
            var loanTemplate = new GetLoanTemplateDto
            {
                LoanAmount = 25000000 //25mil => 25% of assets so +20 
            };

            var userScore = _sut.CalculateCustomerCreditScore(customer.Id, customerBackground, loanTemplate);

            userScore.Should().Be(35);

        }

        [Fact]
        public void CheckCustomerBackground_check_customer_background_properly()
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);

            var customer = new CustomerBuilder(role.Id).Build();
            Save(customer);


            var overdueLoan = new LoanBuilder(customer.Id)
                .WithDurationMonths(1)
                .WithLoanStatus(LoanStatus.Overdue)
                .Build();
            Save(overdueLoan);

            var overdueInstallment = new InstallmentBuilder(overdueLoan.Id)
                .WithDueDate(DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1).AddDays(5)))
                .WithInstallmentStatus(InstallmentStatus.Overdue)
                .WithPaymentAmount(overdueLoan.MonthlyPayment)
                .Build();
            Save(overdueInstallment);

            var actual = _sut.CheckCustomerBackground(customer.Id);

            actual.Should().BeEquivalentTo(new CustomerBackgroundDto
            {
                CustomerPendingLoanRequestCount = 0,
                TotalLoansCount = 1,
                OverdueInstallmentCount = 1
            });
        }

        [Fact]
        public void GetRequestedLoanByCustomerId_get_requested_loan_peoperly()
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var user = new CustomerBuilder(role.Id).Build();
            Save(user);
            var loan = new LoanBuilder(user.Id).Build();
            Save(loan);

            var actual = _sut.GetRequestedLoanByCustomerId(user.Id);

            actual.Should().BeEquivalentTo(new RequestedLoanDto
            {
                CustomerId = user.Id,
                DurationMonths = loan.DurationMonths,
                AnnualInterestRate = loan.AnnualInterestRate,
                LoanId = loan.Id,
                LoanAmount = loan.LoanAmount,
            });
        }

        [Theory]
        [InlineData(65, 0)]
        public void RegisterLoan_register_loan_properly(int customerCreditScore, int PendingLoanRequest)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id).Build();
            Save(customer);
            var dto = new GetLoanTemplateDto
            {
                AnnualInterestRate = 15,
                DurationMonths = 9,
                InstallmentCount = 9,
                LoanAmount = 1000000
            };
            var isCustomerVerified = true;

            var actual = _sut.RegisterLoan(customer.Id, dto, customerCreditScore, PendingLoanRequest ,isCustomerVerified);

            var expected = ReadContext.Set<Loan>().FirstOrDefault(_ => _.Id == actual);
            expected.Should().NotBeNull();
            expected!.Id.Should().Be(actual);
            expected.DurationMonths.Should().Be(9);
            expected.LoanStatus.Should().Be(LoanStatus.Pending);
        }

        [Fact]
        public void RejectLoan_reject_loan_properly()
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var user = new CustomerBuilder(role.Id).Build();
            Save(user);
            var loan = new LoanBuilder(user.Id).WithLoanStatus(LoanStatus.Pending).Build();
            Save(loan);

            _sut.RejectLoan(user.Id);

            var actual = ReadContext.Set<Loan>().FirstOrDefault(_ => _.Id == loan.Id);
            actual!.LoanStatus.Should().Be(LoanStatus.Rejected);

        }

        [Theory]
        [InlineData(-1)]
        public void CalculateCustomerCreditScore_throw_exception_when_customer_not_exist(int fakeCustomerId)
        {
            var customerBackground = new CustomerBackgroundDto
            {
                CustomerPendingLoanRequestCount = 1,
                OverdueInstallmentCount = 1,
                TotalLoansCount = 1,
            };
            var loanTemplate = new GetLoanTemplateDto
            {
                AnnualInterestRate = 1,
                DurationMonths = 1,
                InstallmentCount = 1,
                LoanAmount = 1,
            };

            var actual = () => _sut.CalculateCustomerCreditScore(fakeCustomerId, customerBackground, loanTemplate);

            actual.Should().ThrowExactly<CustomerNotFoundException>();
        }
        [Theory]
        [InlineData(-1)]
        public void CheckCustomerBackground_throw_exception_when_customer_not_exist(int fakeCustomerId)
        {
            var actual = () => _sut.CheckCustomerBackground(fakeCustomerId);

            actual.Should().ThrowExactly<CustomerNotFoundException>();
        }
        [Theory]
        [InlineData(-1)]
        public void GetRequestedLoanByCustomerId_throw_exception_when_customer_not_exist(int fakeCustomerId)
        {
            var actual = () => _sut.GetRequestedLoanByCustomerId(fakeCustomerId);

            actual.Should().ThrowExactly<CustomerNotFoundException>();
        }
        [Theory]
        [InlineData(-1)]
        public void RegisterLoan_throw_exception_when_customer_not_exist(int fakeCustomerId)
        {
            var dto = new GetLoanTemplateDto
            {
                AnnualInterestRate = 1,
                DurationMonths = 1,
                InstallmentCount = 1,
                LoanAmount = 1,
            };

            var actual = () => _sut.RegisterLoan(fakeCustomerId, dto, 0, 0 , true);

            actual.Should().ThrowExactly<CustomerNotFoundException>();
        }
        [Fact]
        public void RegisterLoan_throw_exception_when_customer_is_not_verified()
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id).Build();
            Save(customer);
            var dto = new GetLoanTemplateDto
            {
                AnnualInterestRate = 1,
                DurationMonths = 1,
                InstallmentCount = 1,
                LoanAmount = 1,
            };

            var actual = () => _sut.RegisterLoan(customer.Id, dto, 0, 0, false);

            actual.Should().ThrowExactly<CustomerNotVerifiedException>();
        }

        [Theory]
        [InlineData(1, 65)]
        public void RegisterLoan_throw_exception_when_customer_have_unapproved_loan(int fakePendingLoanRequest, int fakeScore)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id).Build();
            Save(customer);
            var dto = new GetLoanTemplateDto
            {
                AnnualInterestRate = 1,
                DurationMonths = 1,
                InstallmentCount = 1,
                LoanAmount = 1,
            };

            var actual = () => _sut.RegisterLoan(customer.Id, dto,fakeScore , fakePendingLoanRequest , true);

            actual.Should().ThrowExactly<CustomerHaveLoanToApproveException>();
        }

        [Theory]
        [InlineData(-1 , 0)]
        public void RegisterLoan_throw_exception_when_customer_is_not_qualified_for_applying(int fakeScore , int facePendingLoanRequest)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id).Build();
            Save(customer);
            var dto = new GetLoanTemplateDto
            {
                AnnualInterestRate = 1,
                DurationMonths = 1,
                InstallmentCount = 1,
                LoanAmount = 1,
            };

            var actual = () => _sut.RegisterLoan(customer.Id, dto, fakeScore, facePendingLoanRequest, true);

            actual.Should().ThrowExactly<CustomerIsNotQualifiedForApplyingException>();
        }

        [Theory]
        [InlineData(-1)]
        public void RejectLoan_throw_exception_when_customer_not_exist(int fakeCustomerId)
        {
            var actual = () => _sut.RejectLoan(fakeCustomerId);

            actual.Should().ThrowExactly<CustomerNotFoundException>();
        }

        [Fact]
        public void GetCustomerLoans_throw_exception_when_customer_do_not_have_loans()
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id).Build();
            Save(customer);

            var exception =()=> _sut.GetCustomerLoans(customer.Id);

            exception.Should().ThrowExactly<CustomerDoesNotHaveLoan>();

        }

    }
}
