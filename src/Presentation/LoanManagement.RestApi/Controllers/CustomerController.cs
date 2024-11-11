using System.Xml;
using loanManagement.Services.Installments.Contracts.Interfaces;
using loanManagement.Services.Loans.Contracts.DTOs;
using loanManagement.Services.Loans.Contracts.Interfaces;
using loanManagement.Services.LoanTemplates;
using loanManagement.Services.LoanTemplates.Contracts.DTOs;
using loanManagement.Services.LoanTemplates.Contracts.Interfaces;
using loanManagement.Services.Users.Contracts.DTOs;
using loanManagement.Services.Users.Contracts.Interfaces;
using LoanManagement.Application.Loans.RegisterLoanRequest.Contracts;
using LoanManagement.Entities.Users;
using Microsoft.AspNetCore.Mvc;

namespace LoanManagement.RestApi.Controllers
{
    [Route("Api/")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly InstallmentService _installmentService;
        private readonly LoanService _loanService;
        private readonly LoanTemplateService _loanTemplateService;
        private readonly RegisterLoanRequestHandler _registerLoanRequestHandler;

        public CustomerController(
            UserService userService,
            InstallmentService installmentService,
            LoanService loanService,
            LoanTemplateService loanTemplateService,
            RegisterLoanRequestHandler handler)
        {
            _installmentService = installmentService;
            _userService = userService;
            _loanService = loanService;
            _loanTemplateService = loanTemplateService;
            _registerLoanRequestHandler = handler;
        }

        [HttpPut("Loans/Register-Loan-Request/{loanTemplateId}")]
        public void RegisterLoanRequest([FromBody]string email , [FromRoute]int loanTemplateId)
        {
            var customer = _userService.FindByEmail(email);
            _registerLoanRequestHandler.Handle(customer.Id, loanTemplateId);
        }

        [HttpGet("Customer/Loans/Get-All-Loans")] 
        public List<CustomerLoanDto> GetAllCustomerLoans([FromBody] string email)
        {
            var customer = _userService.FindByEmail(email);
            return _loanService.GetCustomerLoans(customer.Id);
        }

        [HttpPatch("Installments/Pay-Installments/{loanId}")]
        public void PayLoanInstallment([FromRoute] int loanId)
        {
            _installmentService.PayLoanInstallment(loanId);
        }
        [HttpPost("Customer/SignUp")]
        public void CustomerSignUp([FromBody] CustomerSignUpDto dto)
        {
            _userService.CustomerSignUp(dto);
        }

        [HttpGet("LoanTemplates/Get-All-Loan-Templates")]
        public List<LoanTemplateDto> GetAllLoanTemplates()
        {
            return _loanTemplateService.GetAllLoanTemplates();
        }

        [HttpPatch("Change-Phone-Number")]
        public void ChangePhoneNumber([FromBody] ChangePhoneNumberDto dto)
        {
            _userService.UpdatePhoneNumber(dto.Email, dto.Password, dto.NewPhoneNumber);
        }

        [HttpPatch("Change-Password")]
        public void ChangePassword([FromBody] ChangePasswordDto dto)
        {
            _userService.ChangePassword(dto.Email, dto.CurrentPassword, dto.NewPassword, dto.NewPasswordConfirmation);
        }

        [HttpDelete("Customer/Delete-Account")]
        public void Delete([FromBody] string email)
        {
            var user = _userService.FindByEmail(email);
            _userService.Delete(user.Id);
        }
        [HttpPatch("Send-Verification-Request")]
        public void SendVerificationRequest([FromBody]SendVerificationRequestDto dto)
        {
            _userService.SendVerificationRequest(dto.Email, dto.Password);
        }

        [HttpPatch("Update-Financial-Assets")]
        public void UpdateFinancialAssets([FromBody]UpdateFinancialAssetsDto dto)
        {
            _userService.UpdateFinancialAssets(dto.Email , dto.Password , dto.NewAssets);
        }

        [HttpPatch("Update-Job-Type")]
        public void UpdateJobType([FromBody]UpdateJobTypeDto dto)
        {
            _userService.UpdateJobType(dto.Email , dto.Password , dto.NewJobType);
        }

        [HttpPatch("Update-Monthly-Income")]
        public void UpdateMonthlyIncome(UpdateMonthlyIncomeDto dto)
        {
            _userService.UpdateMonthlyIncome(dto.Email, dto.Password, dto.NewIncome);
        }


    }
}
