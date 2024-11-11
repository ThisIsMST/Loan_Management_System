using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using loanManagement.Services.Users.Contracts.DTOs;
using loanManagement.Services.Users.Contracts.Interfaces;
using LoanManagement.Entities.Users;
using LoanManagement.Persistence.EF.Users;
using LoanManagement.TestTools.Infrastructure.DataBaseConfig.Integration;
using LoanManagement.TestTools.Roles;
using LoanManagement.TestTools.Users;

namespace LoanManagements.Service.Unit.Tests.Users
{
    public class UserQueryTests : BusinessIntegrationTest
    {
        private readonly UserQuery _sut;

        public UserQueryTests()
        {
            _sut = new EFUserQuery(SetupContext);   
        }

        [Fact]
        public void GetCustomerFinancialData_get_customer_financial_data_properly()
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                .WithMonthlyIncome(100)
                .WithFinancialAssets(110)
                .WithJobType(JobType.SelfEmployed)
                .Build();
            Save(customer);
            var dto = new CustomerFinancialDto
            {
                MonthlyIncome = (decimal)customer.MonthlyIncome,
                FinancialAssets = (decimal)customer.FinancialAssets,
                JobType = (JobType)customer.JobType,
            };


            var actual = _sut.GetCustomerFinancialData(customer.Id);

            actual.Should().BeEquivalentTo(dto);

        }

        [Fact]
        public void GetAllCustomers()
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                .WithFirstName("Test")
                .WithLastName("Test")
                .WithNationalId("123")
                .WithPhoneNumber("123456789")
                .WithEmail("Test@Test.com")
                .WithMonthlyIncome(100)
                .WithCustomerScore(100)
                .WithJobType(JobType.SelfEmployed)
                .WithFinancialAssets(100)
                .WithVerificationStatus(VerificationStatus.Verified)
                .Build();
            Save(customer);

            var actual = _sut.GetAllCustomers();

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(new CustomerDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                NationalId = customer.NationalId,
                PhoneNumber = customer.PhoneNumber,
                Email = customer.Email,
                FinancialAssets = (decimal)customer.FinancialAssets,
                CustomerJobType = (JobType)customer.JobType,
                MonthlyIncome = (decimal)customer.MonthlyIncome,
                VerificationStatus = (VerificationStatus)customer.VerificationStatus,

            });
                
        }
    }
}
