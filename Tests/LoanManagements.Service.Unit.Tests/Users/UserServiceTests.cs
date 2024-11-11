using System.Configuration;
using FluentAssertions;
using loanManagement.Services.Users;
using loanManagement.Services.Users.Contracts.DTOs;
using loanManagement.Services.Users.Contracts.Interfaces;
using loanManagement.Services.Users.Exceptions;
using LoanManagement.Entities.Roles;
using LoanManagement.Entities.Users;
using LoanManagement.Persistence.EF.DataContext;
using LoanManagement.Persistence.EF.Roles;
using LoanManagement.Persistence.EF.Users;
using LoanManagement.TestTools.Infrastructure.DataBaseConfig.Integration;
using LoanManagement.TestTools.Roles;
using LoanManagement.TestTools.Users;
using Microsoft.AspNet.Identity;

namespace LoanManagements.Service.Unit.Tests.Users
{
    public class UserServiceTests : BusinessIntegrationTest
    {
        private readonly UserService _sut;
        public UserServiceTests()
        {
            var roleRepository = new EFRoleRepository(SetupContext);
            var userRepository = new EFUserRepository(SetupContext);
            var unitOfWork = new EFUnitOfWork(SetupContext);
            var userQuery = new EFUserQuery(SetupContext);
            _sut = new UserAppService(roleRepository, userRepository, unitOfWork , userQuery);
        }

        [Fact]
        public void AdminSignUp_add_admin_properly()
        {

            var adminDto = new AdminSignUpDto
            {
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "Test",
                NationalId = "1234567890",
                Password = "password123",
                PhoneNumber = "1234567890"
            };

            var userId = _sut.AdminSignUp(adminDto);

            var actual = ReadContext.Set<User>().Single(_ => _.Id == userId);
            var role = ReadContext.Set<Role>().FirstOrDefault(x => x.Id == actual.RoleId);

            actual.Id.Should().Be(userId);
            var result = new PasswordHasher().VerifyHashedPassword(actual.PasswordHash, adminDto.Password);
            result.Should().Be(PasswordVerificationResult.Success);
            role!.RoleName.Should().Be("admin");
            role.Should().NotBeNull();

        }

        [Fact]
        public void CustomerSignUp_add_customer_properly()
        {
            var customerDto = new CustomerSignUpDto
            {
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "Test",
                NationalId = "1234567890",
                Password = "password123",
                PhoneNumber = "1234567890",
                FinancialAssets = 1000000000,
                JobType = JobType.Employee,
                MonthlyIncome = 10000000,
            };
            var customerId = _sut.CustomerSignUp(customerDto);

            var actual = ReadContext.Set<User>().Single(_ => _.Id == customerId);
            var role = ReadContext.Set<Role>().FirstOrDefault(x => x.Id == actual.RoleId);

            actual.Id.Should().Be(customerId);
            var result = new PasswordHasher().VerifyHashedPassword(actual.PasswordHash, customerDto.Password);
            result.Should().Be(PasswordVerificationResult.Success);
            role!.RoleName.Should().Be("customer");
            role.Should().NotBeNull();

        }
        [Fact]
        public void Delete_delete_user_properly()
        {
            var role = new RoleBuilder().WithAdminRole().Build();
            Save(role);
            var admin = new AdminBuilder(role.Id).Build();
            Save(admin);
            var role2 = new RoleBuilder().WithCustomerRole().Build();
            Save(role2);
            var customer = new CustomerBuilder(role2.Id).Build();
            Save(customer);

            _sut.Delete(admin.Id);
            _sut.Delete(customer.Id);

            var actual = ReadContext.Set<User>().ToList();
            actual.Should().BeEmpty();

        }

        [Theory]
        [InlineData("Test@Test.com", "Test@123", "Test@321")]
        public void ChangePassword_change_password_properly(string fakeEmail, string fakePassword, string newPassword)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var user = new CustomerBuilder(role.Id).WithPassword(fakePassword).WithEmail(fakeEmail).Build();
            Save(user);

            _sut.ChangePassword(fakeEmail, fakePassword, newPassword, newPassword);

            var actual = ReadContext.Set<User>().Single(_ => _.Id == user.Id);
            var result = new PasswordHasher().VerifyHashedPassword(actual.PasswordHash, newPassword);
            result.Should().Be(PasswordVerificationResult.Success);
            actual.VerificationStatus.Should().Be(VerificationStatus.Unverified);
        }
        [Theory]
        [InlineData(100, 10)]
        public void AddLoanToCustomerAssets_add_loan_amount_to_customer_assets_properly(decimal customerAssets, decimal loanAmount)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id).WithFinancialAssets(customerAssets).Build();
            Save(customer);

            _sut.AddLoanToCustomerAssets(customer.Id, loanAmount);

            var actual = ReadContext.Set<User>().Single(_ => _.Id == customer.Id);
            actual.FinancialAssets.Should().Be(customerAssets + loanAmount);
        }
        [Fact]
        public void VerifyCustomer_verify_customer_properly()
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id).WithVerificationStatus(VerificationStatus.Requested).Build();
            Save(customer);

            _sut.VerifyCustomer(customer.Id);

            var actual = ReadContext.Set<User>().Single(_ => _.Id == customer.Id);
            actual.VerificationStatus.Should().Be(VerificationStatus.Verified);
        }

        [Fact]
        public void RejectCustomerVerificationRequest_reject_verification_request_properly()
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id).WithVerificationStatus(VerificationStatus.Requested).Build();
            Save(customer);

            _sut.RejectCustomerVerificationRequest(customer.Id);

            var actual = ReadContext.Set<User>().Single(_ => _.Id == customer.Id);
            actual.VerificationStatus.Should().Be(VerificationStatus.Failed);
        }

        [Fact]
        public void IsCustomerVerified_return_true_when_customer_is_verified()
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id).WithVerificationStatus(VerificationStatus.Verified).Build();
            Save(customer);

            var result = _sut.IsCustomerVerified(customer.Id);

            result.Should().BeTrue();
        }

        [Fact]
        public void IsCustomerVerified_return_true_when_customer_is_unverified()
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id).WithVerificationStatus(VerificationStatus.Unverified).Build();
            Save(customer);

            var result = _sut.IsCustomerVerified(customer.Id);

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("Test@Test.com", "Test@123")]
        public void SendVerificationRequest_send_request_properly(string fakeEmail, string fakePassword)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id).WithEmail(fakeEmail).WithPassword(fakePassword).Build();
            Save(customer);

            _sut.SendVerificationRequest(fakeEmail, fakePassword);

            var actual = ReadContext.Set<User>().Single(_ => _.Id == customer.Id);
            actual.VerificationStatus.Should().Be(VerificationStatus.Requested);

        }

        [Theory]
        [InlineData(75)]
        public void UpdateCustomerCreditScore_update_customer_score_properly(int newFakeScore)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id).WithCustomerScore(55).Build();
            Save(customer);

            _sut.UpdateCustomerCreditScore(customer.Id, newFakeScore);

            var actual = ReadContext.Set<User>().Single(_ => _.Id == customer.Id);
            actual.CustomerScore.Should().Be(newFakeScore);


        }
        [Theory]
        [InlineData("Test@Test.com", "Test@123", "981234567890", "980987654321")]
        public void UpdatePhoneNumber_update_phone_number_properly(string fakeEmail
            , string fakePassword
            , string currentPhoneNumber
            , string newPhoneNumber)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                .WithEmail(fakeEmail)
                .WithPassword(fakePassword)
                .WithPhoneNumber(currentPhoneNumber)
                .Build();
            Save(customer);

            _sut.UpdatePhoneNumber(fakeEmail, fakePassword, newPhoneNumber);

            var actual = ReadContext.Set<User>().SingleOrDefault(_ => _.Id == customer.Id);
            actual.Should().NotBeNull();
            actual!.PhoneNumber.Should().Be(newPhoneNumber);
            actual.VerificationStatus.Should().Be(VerificationStatus.Unverified);
        }

        [Theory]
        [InlineData("Test@Test.com", "Test@123", 100, 101)]
        public void UpdateMonthlyIncome(string fakeEmail, string fakePasswoed, decimal fakeCurrentIncome, decimal fakeNewIncome)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                .WithEmail(fakeEmail)
                .WithPassword(fakePasswoed)
                .WithMonthlyIncome(fakeCurrentIncome)
                .Build();
            Save(customer);

            _sut.UpdateMonthlyIncome(fakeEmail, fakePasswoed, fakeNewIncome);

            var actual = ReadContext.Set<User>().SingleOrDefault(_ => _.Id == customer.Id);
            actual.Should().NotBeNull();
            actual!.MonthlyIncome.Should().Be(fakeNewIncome);
            actual.VerificationStatus.Should().Be(VerificationStatus.Unverified);
        }

        [Theory]
        [InlineData("Test@Test.com", "Test@123", JobType.SelfEmployed, JobType.Employee)]
        public void UpdateJobType_update_customer_job_type_properly(
            string fakeEmail,
            string fakePassword,
            JobType fakeCurrentJob,
            JobType fakeNewJob)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                            .WithEmail(fakeEmail)
                            .WithPassword(fakePassword)
                            .WithJobType(fakeCurrentJob)
                            .Build();
            Save(customer);

            _sut.UpdateJobType(fakeEmail, fakePassword, fakeNewJob);

            var actual = ReadContext.Set<User>().SingleOrDefault(_ => _.Id == customer.Id);
            actual.Should().NotBeNull();
            actual!.JobType.Should().Be(fakeNewJob);
            actual.VerificationStatus.Should().Be(VerificationStatus.Unverified);
        }

        [Theory]
        [InlineData("Test@Test.com", "Test@123", 100, 101)]
        public void UpdateFinancialAssets_update_customer_assets_properly(
            string fakeEmail,
            string fakePassword,
            decimal fakeCurrentAssets,
            decimal fakeNewAssets)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                            .WithEmail(fakeEmail)
                            .WithPassword(fakePassword)
                            .WithFinancialAssets(fakeCurrentAssets)
                            .Build();
            Save(customer);

            _sut.UpdateFinancialAssets(fakeEmail, fakePassword, fakeNewAssets);

            var actual = ReadContext.Set<User>().SingleOrDefault(_ => _.Id == customer.Id);
            actual.Should().NotBeNull();
            actual!.FinancialAssets.Should().Be(fakeNewAssets);
            actual.VerificationStatus.Should().Be(VerificationStatus.Unverified);
        }

        [Theory]
        [InlineData("Test@Test.com")]
        public void AdminSignUp_throw_exception_when_email_is_duplicate(string email)
        {
            var role = new RoleBuilder().WithAdminRole().Build();
            Save(role);
            var admin = new AdminBuilder(role.Id).WithEmail(email).Build();
            Save(admin);
            var dto = new AdminSignUpDto
            {
                Email = email,
                Password = "Test@123",
                FirstName = "Test",
                LastName = "Test",
                NationalId = "123",
                PhoneNumber = "98123456789"
            };

            var exception = () => _sut.AdminSignUp(dto);

            exception.Should().ThrowExactly<DuplicateEmailException>();
        }

        [Theory]
        [InlineData("Test@Test.com")]
        public void CustomerSignUp_throw_exception_when_email_is_duplicate(string email)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id).WithEmail(email).Build();
            Save(customer);
            var dto = new CustomerSignUpDto
            {
                Email = email,
                Password = "Test@123",
                FirstName = "Test",
                LastName = "Test",
                NationalId = "123",
                PhoneNumber = "98123456789",
            };

            var exception = () => _sut.CustomerSignUp(dto);

            exception.Should().ThrowExactly<DuplicateEmailException>();
        }
        [Theory]
        [InlineData(-1)]
        public void Delete_throw_exception_when_user_is_not_exist(int fakeId)
        {
            var exception = () => _sut.Delete(fakeId);

            exception.Should().ThrowExactly<UserNotFoundException>();
        }

        [Theory]
        [InlineData(-1, 0)]
        public void UpdateCustomerCreditScore_throw_exception_when_user_is_not_exist(int fakeId, int fakeScore)
        {
            var exception = () => _sut.UpdateCustomerCreditScore(fakeId, fakeScore);

            exception.Should().ThrowExactly<CustomerNotFoundException>();
        }

        [Theory]
        [InlineData(-1)]
        public void VerifyCustomer_throw_exception_when_user_is_not_exist(int fakeId)
        {
            var exception = () => _sut.VerifyCustomer(fakeId);

            exception.Should().ThrowExactly<CustomerNotFoundException>();
        }

        [Theory]
        [InlineData(-1)]
        public void RejectCustomerVerificationRequest_throw_exception_when_user_is_not_exist(int fakeId)
        {
            var exception = () => _sut.RejectCustomerVerificationRequest(fakeId);

            exception.Should().ThrowExactly<CustomerNotFoundException>();
        }

        [Theory]
        [InlineData(-1)]
        public void IsCustomerVerified_throw_exception_when_user_is_not_exist(int fakeId)
        {
            var exception = () => _sut.IsCustomerVerified(fakeId);

            exception.Should().ThrowExactly<CustomerNotFoundException>();
        }

        [Theory]
        [InlineData("Test@Test.com", "Test@123", "Test@321", "Test@12")]
        public void ChangePassword_throw_exception_when_new_password_and_confirmation_does_not_match(string fakeEmail, string fakePassword, string fakeNewPassword, string fakeConfirmation)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                            .WithPassword("Test@123")
                            .Build();
            Save(customer);

            var exception = () => _sut.ChangePassword(fakeEmail, fakePassword, fakeNewPassword, fakeConfirmation);

            exception.Should().ThrowExactly<PasswordConfirmationMismatchException>();
        }

        [Theory]
        [InlineData("Test@Test.com", "Test@Mail.com", "Test@123", "Test@321", "Test@321")]
        public void ChangePassword_throw_exception_when_email_is_incorrect(string fakeEmail
            , string fakeWrongEmail
            , string fakePassword
            , string fakeNewPassword
            , string fakeConfirmation)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                            .WithPassword(fakePassword)
                            .WithEmail(fakeEmail)
                            .Build();
            Save(customer);

            var exception = () => _sut.ChangePassword(fakeWrongEmail, fakePassword, fakeNewPassword, fakeConfirmation);

            exception.Should().ThrowExactly<InvalidEmailOrPasswordException>();
        }

        [Theory]
        [InlineData("Test@Test.com", "Test@123", "Test@2024", "Test@321", "Test@321")]
        public void ChangePassword_throw_exception_when_password_is_incorrect(string fakeEmail
            , string fakeWrongPassword
            , string fakePassword
            , string fakeNewPassword
            , string fakeConfirmation)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                            .WithPassword(fakePassword)
                            .WithEmail(fakeEmail)
                            .Build();
            Save(customer);

            var exception = () => _sut.ChangePassword(fakeEmail, fakeWrongPassword, fakeNewPassword, fakeConfirmation);

            exception.Should().ThrowExactly<InvalidEmailOrPasswordException>();
        }

        [Fact]
        public void VerifyCustomer_throw_exception_when_user_is_already_verified()
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                            .WithVerificationStatus(VerificationStatus.Verified)
                            .Build();
            Save(customer);

            var exception = () => _sut.VerifyCustomer(customer.Id);

            exception.Should().ThrowExactly<CustomerAlreadyVerifiedException>();
        }

        [Fact]
        public void VerifyCustomer_throw_exception_when_user_did_not_sent_verification_request()
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                            .WithVerificationStatus(VerificationStatus.Unverified)
                            .Build();
            Save(customer);

            var exception = () => _sut.VerifyCustomer(customer.Id);

            exception.Should().ThrowExactly<CustomerDidNotSentVerificationRequest>();
        }


        [Fact]
        public void RejectCustomerVerificationRequest_throw_exception_when_user_is_already_verified()
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                            .WithVerificationStatus(VerificationStatus.Verified)
                            .Build();
            Save(customer);

            var exception = () => _sut.RejectCustomerVerificationRequest(customer.Id);

            exception.Should().ThrowExactly<CustomerAlreadyVerifiedException>();
        }

        [Fact]
        public void RejectCustomerVerificationRequest_throw_exception_when_user_did_not_sent_verification_request()
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                            .WithVerificationStatus(VerificationStatus.Unverified)
                            .Build();
            Save(customer);

            var exception = () => _sut.RejectCustomerVerificationRequest(customer.Id);

            exception.Should().ThrowExactly<CustomerDidNotSentVerificationRequest>();
        }

        [Theory]
        [InlineData("981234567890", "Test@Test1.com", "Test@Test2.com", "Test@123")]
        public void UpdatePhoneNumber_throw_exception_when_email_is_incorrect(string fakeNumber,
            string fakeEmail,
            string fakeWrongEmail,
            string fakePassword)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                                        .WithEmail(fakeEmail)
                                        .Build();
            Save(customer);

            var exception = () => _sut.UpdatePhoneNumber(fakeWrongEmail, fakePassword, fakeNumber);

            exception.Should().ThrowExactly<InvalidEmailOrPasswordException>();

        }

        [Theory]
        [InlineData("981234567890", "Test@Test1.com", "Test@321", "Test@123")]
        public void UpdatePhoneNumber_throw_exception_when_password_is_incorrect(string fakeNumber,
           string fakeEmail,
           string fakeWrongPassword,
           string fakePassword)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                                        .WithEmail(fakeEmail)
                                        .WithPassword(fakePassword)
                                        .Build();
            Save(customer);

            var exception = () => _sut.UpdatePhoneNumber(fakeEmail, fakeWrongPassword, fakeNumber);

            exception.Should().ThrowExactly<InvalidEmailOrPasswordException>();

        }

        [Theory]
        [InlineData("981234567890", "Test@Test1.com", "Test@123")]
        public void UpdatePhoneNumber_throw_exception_when_phone_number_is_duplicate(string fakeNumber,
           string fakeEmail,
           string fakePassword)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                                        .WithPhoneNumber(fakeNumber)
                                        .WithEmail(fakeEmail)
                                        .WithPassword(fakePassword)
                                        .Build();
            Save(customer);

            var exception = () => _sut.UpdatePhoneNumber(fakeEmail, fakePassword, fakeNumber);

            exception.Should().ThrowExactly<DuplicatePhoneNumberException>();

        }

        [Theory]
        [InlineData("Test@Test1.com", "Test@Test2.com", "Test@123")]
        public void SendVerificationRequest_throw_exception_when_email_is_incorrect(
        string fakeEmail,
        string fakeWrongEmail,
        string fakePassword)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                                        .WithEmail(fakeEmail)
                                        .Build();
            Save(customer);

            var exception = () => _sut.SendVerificationRequest(fakeWrongEmail, fakePassword);

            exception.Should().ThrowExactly<InvalidEmailOrPasswordException>();

        }

        [Theory]
        [InlineData("Test@Test1.com", "Test@321", "Test@123")]
        public void SendVerificationRequest_throw_exception_when_password_is_incorrect(
           string fakeEmail,
           string fakeWrongPassword,
           string fakePassword)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                                        .WithEmail(fakeEmail)
                                        .WithPassword(fakePassword)
                                        .Build();
            Save(customer);

            var exception = () => _sut.SendVerificationRequest(fakeEmail, fakeWrongPassword);

            exception.Should().ThrowExactly<InvalidEmailOrPasswordException>();

        }

        [Fact]
        public void SendVerificationRequest_throw_exception_when_user_is_already_verified()
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                            .WithPassword("Test@123")
                            .WithVerificationStatus(VerificationStatus.Verified)
                            .Build();
            Save(customer);

            var exception = () => _sut.SendVerificationRequest(customer.Email, "Test@123");

            exception.Should().ThrowExactly<CustomerAlreadyVerifiedException>();
        }

        [Fact]
        public void SendVerificationRequest_throw_exception_when_user_is_already_sent_verification_request()
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                            .WithPassword("Test@123")
                            .WithVerificationStatus(VerificationStatus.Requested)
                            .Build();
            Save(customer);

            var exception = () => _sut.SendVerificationRequest(customer.Email, "Test@123");

            exception.Should().ThrowExactly<CustomerRequestAlreadySentException>();
        }

        [Theory]
        [InlineData(100, "Test@Test1.com", "Test@Test2.com", "Test@123")]
        public void UpdateMonthlyIncome_throw_exception_when_email_is_incorrect(decimal fakeIncome,
        string fakeEmail,
        string fakeWrongEmail,
        string fakePassword)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                                        .WithEmail(fakeEmail)
                                        .Build();
            Save(customer);

            var exception = () => _sut.UpdateMonthlyIncome(fakeWrongEmail, fakePassword, fakeIncome);

            exception.Should().ThrowExactly<InvalidEmailOrPasswordException>();

        }

        [Theory]
        [InlineData(100, "Test@Test1.com", "Test@321", "Test@123")]
        public void UpdateMonthlyIncome_throw_exception_when_password_is_incorrect(decimal fakeIncome,
           string fakeEmail,
           string fakeWrongPassword,
           string fakePassword)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                                        .WithEmail(fakeEmail)
                                        .WithPassword(fakePassword)
                                        .Build();
            Save(customer);

            var exception = () => _sut.UpdateMonthlyIncome(fakeEmail, fakeWrongPassword, fakeIncome);

            exception.Should().ThrowExactly<InvalidEmailOrPasswordException>();

        }

        [Theory]
        [InlineData(100, "Test@Test1.com", "Test@123")]
        public void UpdateMonthlyIncome_throw_exception_when_phone_number_is_duplicate(decimal fakeIncome,
           string fakeEmail,
           string fakePassword)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                                        .WithMonthlyIncome(fakeIncome)
                                        .WithEmail(fakeEmail)
                                        .WithPassword(fakePassword)
                                        .Build();
            Save(customer);

            var exception = () => _sut.UpdateMonthlyIncome(fakeEmail, fakePassword, fakeIncome);

            exception.Should().ThrowExactly<DuplicateMonthlyIncomeException>();

        }

        [Theory]
        [InlineData(JobType.Unemployed, "Test@Test1.com", "Test@Test2.com", "Test@123")]
        public void UpdateJobType_throw_exception_when_email_is_incorrect(JobType fakeJob,
        string fakeEmail,
        string fakeWrongEmail,
        string fakePassword)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                                        .WithEmail(fakeEmail)
                                        .Build();
            Save(customer);

            var exception = () => _sut.UpdateJobType(fakeWrongEmail, fakePassword, fakeJob);

            exception.Should().ThrowExactly<InvalidEmailOrPasswordException>();

        }

        [Theory]
        [InlineData(JobType.Unemployed, "Test@Test1.com", "Test@321", "Test@123")]
        public void UpdateJobType_throw_exception_when_password_is_incorrect(JobType fakeJob,
           string fakeEmail,
           string fakeWrongPassword,
           string fakePassword)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                                        .WithEmail(fakeEmail)
                                        .WithPassword(fakePassword)
                                        .Build();
            Save(customer);

            var exception = () => _sut.UpdateJobType(fakeEmail, fakeWrongPassword, fakeJob);

            exception.Should().ThrowExactly<InvalidEmailOrPasswordException>();

        }

        [Theory]
        [InlineData(JobType.Unemployed, "Test@Test1.com", "Test@123")]
        public void UpdateJobType_throw_exception_when_job_type_is_duplicate(JobType fakeJob,
           string fakeEmail,
           string fakePassword)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                                        .WithEmail(fakeEmail)
                                        .WithPassword(fakePassword)
                                        .WithJobType(fakeJob)
                                        .Build();
            Save(customer);

            var exception = () => _sut.UpdateJobType(fakeEmail, fakePassword, fakeJob);

            exception.Should().ThrowExactly<DuplicateJobTypeException>();

        }
        [Theory]
        [InlineData(100, "Test@Test1.com", "Test@Test2.com", "Test@123")]
        public void UpdateFinancialAssets_throw_exception_when_email_is_incorrect(decimal fakeAssets,
        string fakeEmail,
        string fakeWrongEmail,
        string fakePassword)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                                        .WithEmail(fakeEmail)
                                        .Build();
            Save(customer);

            var exception = () => _sut.UpdateFinancialAssets(fakeWrongEmail, fakePassword, fakeAssets);

            exception.Should().ThrowExactly<InvalidEmailOrPasswordException>();

        }

        [Theory]
        [InlineData(100, "Test@Test1.com", "Test@321", "Test@123")]
        public void UpdateFinancialAssets_throw_exception_when_password_is_incorrect(decimal fakeAssets,
           string fakeEmail,
           string fakeWrongPassword,
           string fakePassword)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                                        .WithEmail(fakeEmail)
                                        .WithPassword(fakePassword)
                                        .Build();
            Save(customer);

            var exception = () => _sut.UpdateFinancialAssets(fakeEmail, fakeWrongPassword, fakeAssets);

            exception.Should().ThrowExactly<InvalidEmailOrPasswordException>();

        }

        [Theory]
        [InlineData(100, "Test@Test1.com", "Test@123")]
        public void UpdateFinancialAssets_throw_exception_when_assets_is_duplicate(decimal fakeAssets,
           string fakeEmail,
           string fakePassword)
        {
            var role = new RoleBuilder().WithCustomerRole().Build();
            Save(role);
            var customer = new CustomerBuilder(role.Id)
                                        .WithFinancialAssets(fakeAssets)
                                        .WithEmail(fakeEmail)
                                        .WithPassword(fakePassword)
                                        .Build();
            Save(customer);

            var exception = () => _sut.UpdateFinancialAssets(fakeEmail, fakePassword, fakeAssets);

            exception.Should().ThrowExactly<DuplicateFinancialAssetsException>();

        }

        [Theory]
        [InlineData("Test@Test.com")]
        public void FindByEmail_find_user_properly(string email)
        {
            var role = new RoleBuilder().WithAdminRole().Build();
            Save(role);
            var admin = new AdminBuilder(role.Id).WithEmail(email).Build();
            Save(admin);

            var actual = _sut.FindByEmail(email);

            actual.Should().NotBeNull();
        }
        [Theory]
        [InlineData("Test@Test.com" , "Wrong@Test.com")]
        public void FindByEmail_throw_exception_when_user_not_found(string email , string wrongEmail)
        {
            var role = new RoleBuilder().WithAdminRole().Build();
            Save(role);
            var admin = new AdminBuilder(role.Id).WithEmail(email).Build();
            Save(admin);

            var exception = () => _sut.FindByEmail(wrongEmail);

            exception.Should().ThrowExactly<UserNotFoundException>();
        }

        [Fact]
        public void GetAllCustomers_get_all_customers_properly()
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
        [Fact]
        public void GetAllCustomers_throw_exception_when_no_customer_signed_up()
        {

            var actual =()=> _sut.GetAllCustomers();

            actual.Should().ThrowExactly<NoCustomerSignedUpYetException>();
        }
    }
}

