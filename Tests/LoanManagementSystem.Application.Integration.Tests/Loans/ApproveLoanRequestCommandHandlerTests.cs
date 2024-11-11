using loanManagement.Services.Installments.Contracts.Interfaces;
using loanManagement.Services.Loans.Contracts.DTOs;
using loanManagement.Services.Loans.Contracts.Interfaces;
using loanManagement.Services.UnitOfWorks;
using loanManagement.Services.Users.Contracts.Interfaces;
using LoanManagement.Application.Loans.ApplyLoanRequest;
using LoanManagement.Persistence.EF.DataContext;
using LoanManagement.TestTools.Infrastructure.DataBaseConfig.Integration;
using LoanManagement.TestTools.Roles;
using LoanManagement.TestTools.Users;
using Moq;
using Xunit;


namespace LoanManagementSystem.Application.Integration.Tests.Loans
{
    public class ApproveLoanRequestCommandHandlerTests : BusinessIntegrationTest
    {
        private readonly Mock<LoanService> _loanService;
        private readonly Mock<UserService> _userService;
        private readonly Mock<InstallmentService> _installmentService;
        private readonly ApproveLoanRequestCommandHandler _sut;
        private readonly Mock<UnitOfWork> _unitOfWork;

        public ApproveLoanRequestCommandHandlerTests()
        {
            _loanService = new Mock<LoanService>();
            _userService = new Mock<UserService>();
            _installmentService = new Mock<InstallmentService>();
            _unitOfWork = new Mock<UnitOfWork>();

            _sut = new ApproveLoanRequestCommandHandler(
                _loanService.Object,
                _userService.Object,
                _installmentService.Object,
                _unitOfWork.Object);
        }

        [Theory]
        [InlineData(20 , 24 , 1 , 100000 , 1)]
        public void Handle_approves_loan_and_schedules_installments(
            int fakeInterestRate,
            int fakeDuration ,
            int fakeCustomerId,
            int fakeLoanAmount,
            int fakeLoanId
            )
        {
            var tempDto = new RequestedLoanDto
            {
                AnnualInterestRate = fakeInterestRate,
                DurationMonths = fakeDuration,
                CustomerId = fakeCustomerId,
                LoanAmount = fakeLoanAmount,
                LoanId = fakeLoanId,
            };

            _loanService.Setup(s => s.GetRequestedLoanByCustomerId(tempDto.CustomerId)).Returns(tempDto);

            _sut.Handle(tempDto.CustomerId);

            _loanService.Verify(s => s.GetRequestedLoanByCustomerId(tempDto.CustomerId));
            _loanService.Verify(s => s.ApproveLoan(tempDto.LoanId));
            _userService.Verify(s => s.AddLoanToCustomerAssets(tempDto.CustomerId, tempDto.LoanAmount));
            _installmentService.Verify(s => s.ScheduleLoanInstallments(tempDto));

        }
    }
}
