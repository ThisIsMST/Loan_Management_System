using loanManagement.Services.Loans.Contracts.Interfaces;
using loanManagement.Services.LoanTemplates.Contracts.DTOs;
using loanManagement.Services.LoanTemplates.Contracts.Interfaces;
using loanManagement.Services.Users.Contracts.DTOs;
using loanManagement.Services.Users.Contracts.Interfaces;
using LoanManagement.Application.Loans.ApplyLoanRequest.Contracts;
using LoanManagement.Entities.Users;
using Microsoft.AspNetCore.Mvc;

namespace LoanManagement.RestApi.Controllers
{
    [Route("Api/Admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly LoanService _loanService;
        private readonly LoanTemplateService _loanTemplateService;
        private readonly ApproveLoanRequestHandler _approveLoanRequestHandler;

        public AdminController(
            UserService userService,
            LoanService loanService,
            LoanTemplateService loanTemplateService,
            ApproveLoanRequestHandler approveLoanRequestHandler)
        {
            _userService = userService;
            _loanService = loanService;
            _loanTemplateService = loanTemplateService;
            _approveLoanRequestHandler = approveLoanRequestHandler;
        }

        [HttpPost("SignUp")]
        public void AdminSignUp([FromQuery] AdminSignUpDto dto)
        {
            _userService.AdminSignUp(dto);
        }
        [HttpPost("LoanTemplates/Create-New-Loan-Template")]
        public void CreateLoanTemplate([FromBody] AddLoanTemplateDto dto)
        {
            _loanTemplateService.Create(dto);
        }
        [HttpGet("LoanTemplates/Get-All-Loan-Templates")]
        public List<LoanTemplateDto> GetAllLoanTemplates()
        {
            return _loanTemplateService.GetAllLoanTemplates();
        }

        [HttpGet("Customers/Get-All-Customers")]
        public List<CustomerDto> GetAllCustomers()
        {
            return _userService.GetAllCustomers();
            
        }

        [HttpPut("LoanTemplates/Update-Loan-Template/{loanTemplateId}")]
        public void Update([FromRoute] int loanTemplateId, [FromBody] UpdateLoanTemplateDto dto)
        {
            _loanTemplateService.Update(loanTemplateId, dto);
        }

        [HttpDelete("LoanTemplates/Delete-Loan-Template/{loanTemplateId}")]
        public void Delete([FromRoute] int loanTemplateId)
        {
            _loanTemplateService.Delete(loanTemplateId);
        }

        [HttpPatch("Loans/Approve-Loan/{customerId}")]
        public void ApproveLoan([FromRoute] int customerId)
        {
            _approveLoanRequestHandler.Handle(customerId);
        }

        [HttpPatch("Loans/Reject-Loan/{customerId}")]
        public void RejectLoan([FromRoute] int customerId)
        {
            _loanService.RejectLoan(customerId);
        }

        [HttpPatch("Verify-Customer/{customerId}")]
        public void VerifyCustomer([FromRoute] int customerId)
        {
            _userService.VerifyCustomer(customerId);
        }

        [HttpPatch("Reject-Customer/{customerId}")]
        public void RejectCustomerVerificationRequest([FromRoute] int customerId)
        {
            _userService.RejectCustomerVerificationRequest(customerId);
        }

        [HttpPatch("Change-Phone-Number")]
        public void ChangePhoneNumber([FromBody] ChangePhoneNumberDto dto)
        {
            _userService.UpdatePhoneNumber(dto.Email, dto.Password, dto.NewPhoneNumber);
        }


        [HttpPatch("Change-Password")]
        public void ChangePassword([FromBody]ChangePasswordDto dto)
        {
            _userService.ChangePassword(dto.Email, dto.CurrentPassword, dto.NewPassword, dto.NewPasswordConfirmation);
        }

        [HttpDelete("Delete-Account")]
        public void Delete([FromBody] string email)
        {
            var user = _userService.FindByEmail(email);
            _userService.Delete(user.Id);
        }
    }

}
