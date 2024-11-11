using loanManagement.Services.Loans.Contracts.DTOs;
using loanManagement.Services.Loans.Contracts.Interfaces;
using loanManagement.Services.LoanTemplates.Contracts.DTOs;
using loanManagement.Services.LoanTemplates.Contracts.Interfaces;
using loanManagement.Services.UnitOfWorks;
using loanManagement.Services.Users.Contracts.Interfaces;
using LoanManagement.Application.Loans.RegisterLoanRequest;
using LoanManagement.Persistence.EF.DataContext;
using LoanManagement.TestTools.Infrastructure.DataBaseConfig.Integration;
using Moq;

namespace LoanManagementSystem.Application.Integration.Tests.Loans
{
    public class RegisterLoanRequestCommandHandlerTests : BusinessIntegrationTest
    {
        private readonly RegisterLoanRequestCommandHandler _sut;
        private readonly Mock<LoanService> _loanService;
        private readonly Mock<LoanTemplateService> _loanTemplateService;
        private readonly Mock<UserService> _userService;
        private readonly Mock<UnitOfWork> _unitOfWork;

        public RegisterLoanRequestCommandHandlerTests()
        {
            _loanService = new Mock<LoanService>();
            _loanTemplateService = new Mock<LoanTemplateService>();
            _userService = new Mock<UserService>();
            _unitOfWork = new Mock<UnitOfWork>();

            _sut = new RegisterLoanRequestCommandHandler(
                _loanService.Object,
                _loanTemplateService.Object,
                _userService.Object,
                _unitOfWork.Object);
        }
        /*
         *  var AppliedLoan = loanTemplateService.GetLoanTemplateData(loanTemplateId);
                var CustomerBackground = loanService.CheckCustomerBackground(customerId);
                var customerCreditScore = loanService.CalculateCustomerCreditScore(customerId,
                    CustomerBackground,
                    AppliedLoan);

                loanService.RegisterLoan(customerId,
                    AppliedLoan,
                    customerCreditScore,
                    CustomerBackground.CustomerPendingLoanRequestCount);
                userService.UpdateCustomerCreditScore(customerId, customerCreditScore);

         */

        [Theory]
        [InlineData(1, 2, 20, 24, 24, 1000000, 1, 0, 24, 65)]
        public void Handle_apply_a_loan_request(int fakeCustomerId
            , int fakeLoanTemplateId
            , int fakeInterest
            , int fakeDuration
            , int fakeInstallmentCount
            , int fakeAmount
            , int fakePendingLoanCount
            , int fakeOverdueInstallmentCount
            , int fakeLoansCount
            , int fakeScore)
        {
            var tempLoanTemplateDto = new GetLoanTemplateDto
            {
                AnnualInterestRate = fakeInterest,
                DurationMonths = fakeDuration,
                InstallmentCount = fakeInstallmentCount,
                LoanAmount = fakeAmount,
            };
            var tempCustomerBackgroundDto = new CustomerBackgroundDto
            {
                CustomerPendingLoanRequestCount = fakePendingLoanCount,
                OverdueInstallmentCount = fakeOverdueInstallmentCount,
                TotalLoansCount = fakeLoansCount
            };
            var isCustomerVerified = true;

            _userService.Setup(s=>s.IsCustomerVerified(fakeCustomerId)).Returns(isCustomerVerified);
            _loanTemplateService.Setup(s => s.GetLoanTemplateData(fakeLoanTemplateId)).Returns(tempLoanTemplateDto);
            _loanService.Setup(s => s.CheckCustomerBackground(fakeCustomerId)).Returns(tempCustomerBackgroundDto);
            _loanService.Setup(s => s.CalculateCustomerCreditScore(fakeCustomerId, tempCustomerBackgroundDto, tempLoanTemplateDto))
                .Returns(fakeScore);



            _sut.Handle(fakeCustomerId, fakeLoanTemplateId);

            _loanTemplateService.Verify(s => s.GetLoanTemplateData(fakeLoanTemplateId));
            _loanService.Verify(s => s.CheckCustomerBackground(fakeCustomerId));
            _loanService.Verify(s => s.CalculateCustomerCreditScore(fakeCustomerId, tempCustomerBackgroundDto, tempLoanTemplateDto));
            _loanService.Verify(s => s.RegisterLoan(fakeCustomerId, tempLoanTemplateDto, fakeScore, fakePendingLoanCount , isCustomerVerified));
            _userService.Verify(s => s.UpdateCustomerCreditScore(fakeCustomerId, fakeScore));

        }

    }
}
