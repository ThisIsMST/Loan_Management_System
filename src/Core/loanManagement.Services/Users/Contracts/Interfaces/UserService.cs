using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using loanManagement.Services.Users.Contracts.DTOs;
using LoanManagement.Entities.Users;

namespace loanManagement.Services.Users.Contracts.Interfaces
{
    public interface UserService
    {
        int AdminSignUp(AdminSignUpDto dto);
        void UpdateCustomerCreditScore(int customerId, int customerCreditScore);
        void AddLoanToCustomerAssets(int customerId, decimal loanAmount);
        void SendVerificationRequest(string Email, string password);
        void UpdateFinancialAssets(string email, string password, decimal newAssets);
        void UpdateJobType(string email, string password, JobType newJobType);
        void UpdateMonthlyIncome(string email, string password, decimal newIncome);
        void UpdatePhoneNumber(string email, string password, string phoneNumber);
        int CustomerSignUp(CustomerSignUpDto dto);
        void VerifyCustomer(int customerId);
        void RejectCustomerVerificationRequest(int customerId);
        bool IsCustomerVerified(int customerId);
        void Delete(int id);
        void ChangePassword(string email, string currentPassword, string newPassword, string newPasswordConfirmation);
        User FindByEmail(string email);
        List<CustomerDto> GetAllCustomers();
    }
}
