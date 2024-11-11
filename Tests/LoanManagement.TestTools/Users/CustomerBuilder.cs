using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoanManagement.Entities.Users;
using Microsoft.AspNet.Identity;

namespace LoanManagement.TestTools.Users
{
  
    public class CustomerBuilder
    {  
        private readonly User _customer;
        public CustomerBuilder(int roleId)
        {
            _customer = new User
            {
                FirstName = "Test",
                LastName = "Test",
                NationalId = "1234567890",
                Email = "test@test.com",
                PasswordHash = new PasswordHasher().HashPassword("Test@123"),
                PhoneNumber = "08123456789",
                MonthlyIncome = 50000,
                JobType = JobType.Unemployed,
                FinancialAssets = 100000,
                VerificationStatus = VerificationStatus.Unverified,
                CustomerScore = 70,
                RoleId = roleId
            };
        }
        public CustomerBuilder WithFirstName(string firstName)
        {
            _customer.FirstName = firstName;
            return this;
        }
        public CustomerBuilder WithLastName(string lastName)
        {
            _customer.LastName = lastName;
            return this;
        }
        public CustomerBuilder WithNationalId(string nationalId)
        {
            _customer.NationalId = nationalId;
            return this;
        }
        public CustomerBuilder WithEmail(string email)
        {
            _customer.Email = email;
            return this;
        }
        public CustomerBuilder WithPassword(string password)
        {
            _customer.PasswordHash = new PasswordHasher().HashPassword(password);
            return this;
        }
        public CustomerBuilder WithPhoneNumber(string phoneNumber)
        {
            _customer.PhoneNumber = phoneNumber;
            return this;
        }
        public CustomerBuilder WithMonthlyIncome(decimal monthlyIncome)
        {
            _customer.MonthlyIncome = monthlyIncome;
            return this;
        }
        public CustomerBuilder WithJobType(JobType jobType)
        {
            _customer.JobType = jobType;
            return this;
        }
        public CustomerBuilder WithFinancialAssets(decimal financialAssets)
        {
            _customer.FinancialAssets = financialAssets;
            return this;
        }
        public CustomerBuilder WithVerificationStatus(VerificationStatus verificationStatus)
        {
            _customer.VerificationStatus = verificationStatus;
            return this;
        }
        public CustomerBuilder WithCustomerScore(int customerScore)
        {
            _customer.CustomerScore = customerScore;
            return this;
        }
        public User Build() => _customer;
        
    }
}
