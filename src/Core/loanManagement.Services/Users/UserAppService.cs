using loanManagement.Services.Roles.Contracts.Interfaces;
using loanManagement.Services.UnitOfWorks;
using loanManagement.Services.Users.Contracts.DTOs;
using loanManagement.Services.Users.Contracts.Interfaces;
using loanManagement.Services.Users.Exceptions;
using LoanManagement.Entities.Users;
using Microsoft.AspNet.Identity;

namespace loanManagement.Services.Users
{
    public class UserAppService(RoleRepository roleRepository,
        UserRepository repository,
        UnitOfWork unitOfWork ,
        UserQuery userQuery) : UserService
    {
        public void AddLoanToCustomerAssets(int customerId, decimal loanAmount)
        {
            repository.AddLoanToCustomerAssets(customerId, loanAmount);
            unitOfWork.Save();
        }
        public int AdminSignUp(AdminSignUpDto dto)
        {
            var roleId = roleRepository.AddAdmin();
            if (repository.IsEmailDuplicate(dto.Email))
            {
                throw new DuplicateEmailException();
            }
            var user = new User
            {
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                NationalId = dto.NationalId,
                PasswordHash = new PasswordHasher().HashPassword(dto.Password),
                PhoneNumber = dto.PhoneNumber,
                RoleId = roleId
            };
            repository.Add(user);
            unitOfWork.Save();
            return user.Id;

        }
        public void Delete(int userId)
        {
            if (!repository.IsExistById(userId))
            {
                throw new UserNotFoundException();
            }
            var user = repository.FindById(userId);
            repository.Delete(user!);
            unitOfWork.Save();
        }
        public List<CustomerDto> GetAllCustomers()
        {
            var customers = userQuery.GetAllCustomers();
            if (customers.Count == 0)
            {
                throw new NoCustomerSignedUpYetException();
            }
            return customers;
        }
        public void UpdateCustomerCreditScore(int customerId, int customerCreditScore)
        {

            var user = repository.FindById(customerId);
            if (user == null)
            {
                throw new CustomerNotFoundException();
            }
            user.CustomerScore = customerCreditScore;
            repository.Update(user);
            unitOfWork.Save();
        }

        public User FindByEmail(string email)
        {
            var user = repository.FindByEmail(email);
            if (user == null)
            {
                throw new UserNotFoundException();
            }
            return user;

        }

        public void ChangePassword(string email , string currentPassword , string newPassword , string newPasswordConfirmation)
        {
            
            if (newPassword != newPasswordConfirmation)
            {
                throw new PasswordConfirmationMismatchException();
            }
            var user = repository.FindByEmail(email);
            if (user == null)
            {
                throw new InvalidEmailOrPasswordException();
            }
            var passwordHasher = new PasswordHasher();
            var emailVerificationResult = passwordHasher.VerifyHashedPassword(user.PasswordHash, currentPassword);
            if (emailVerificationResult != PasswordVerificationResult.Success)
            {
                throw new InvalidEmailOrPasswordException();
            }
            var userRole = roleRepository.GetRoleByEmail(email);
            if(userRole == "customer")
            {
                user.VerificationStatus = VerificationStatus.Unverified;
            }
            user.PasswordHash = new PasswordHasher().HashPassword(newPassword); 
            repository.Update(user);
            unitOfWork.Save();

        }

        public void VerifyCustomer(int customerId)
        {
            var user = repository.FindById(customerId);
            if (user == null) 
            {
                throw new CustomerNotFoundException();
            }
            if (user.VerificationStatus == VerificationStatus.Verified)
            {
                throw new CustomerAlreadyVerifiedException();
            }
            if (user.VerificationStatus != VerificationStatus.Requested)
            {
                throw new CustomerDidNotSentVerificationRequest();
            }          
            user.VerificationStatus = VerificationStatus.Verified;
            repository.Update(user);
            unitOfWork.Save();
        }
        public void RejectCustomerVerificationRequest(int customerId)
        {
            var user = repository.FindById(customerId);
            if (user == null)
            {
                throw new CustomerNotFoundException();
            }
            if (user.VerificationStatus == VerificationStatus.Verified)
            {
                throw new CustomerAlreadyVerifiedException();
            }
            if (user.VerificationStatus != VerificationStatus.Requested)
            {
                throw new CustomerDidNotSentVerificationRequest();
            }
            user.VerificationStatus = VerificationStatus.Failed;
            repository.Update(user);
            unitOfWork.Save();
        }
        public bool IsCustomerVerified(int customerId)
        {
            var user = repository.FindById(customerId);
            if (user == null)
            {
                throw new CustomerNotFoundException();
            }
            else if (user.VerificationStatus != VerificationStatus.Verified)
            {
                return false;
            }
            return true;
        }
        public void UpdatePhoneNumber(string email, string password, string phoneNumber)
        {
            var user = repository.FindByEmail(email);
            if (user == null)
            {
                throw new InvalidEmailOrPasswordException();
            }
            var passwordHasher = new PasswordHasher();
            var emailVerificationResult = passwordHasher.VerifyHashedPassword(user.PasswordHash, password);
            if (emailVerificationResult != PasswordVerificationResult.Success)
            {
                throw new InvalidEmailOrPasswordException();
            }
            if (phoneNumber == user.PhoneNumber)
            {
                throw new DuplicatePhoneNumberException();
            }
            var userRole = roleRepository.GetRoleByEmail(email);
            if (userRole == "customer")
            {
                user.VerificationStatus = VerificationStatus.Unverified;
            }
            user.PhoneNumber = phoneNumber;
            repository.Update(user);
            unitOfWork.Save();

        }

        #region CustomerSpecificPanel
        public void SendVerificationRequest(string email, string password)
        {
            var user = repository.FindByEmail(email);
            if (user == null)
            {
                throw new InvalidEmailOrPasswordException();
            }
            var passwordHasher = new PasswordHasher();
            var emailVerificationResult = passwordHasher.VerifyHashedPassword(user.PasswordHash, password);
            if (emailVerificationResult != PasswordVerificationResult.Success)
            {
                throw new InvalidEmailOrPasswordException();
            }
            if (user.VerificationStatus == VerificationStatus.Requested)
            {
                throw new CustomerRequestAlreadySentException();
            }
            else if (user.VerificationStatus == VerificationStatus.Verified)
            {
                throw new CustomerAlreadyVerifiedException();
            }
            user.VerificationStatus = VerificationStatus.Requested;
            repository.Update(user);
            unitOfWork.Save();

        }
        public int CustomerSignUp(CustomerSignUpDto dto)
        {
            var roleId = roleRepository.AddCustomer();
            if (repository.IsEmailDuplicate(dto.Email))
            {
                throw new DuplicateEmailException();
            }
            var user = new User
            {
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                NationalId = dto.NationalId,
                CustomerScore = dto.CustomerScore,
                VerificationStatus = dto.VerificationStatus,
                FinancialAssets = dto.FinancialAssets,
                JobType = dto.JobType,
                MonthlyIncome = dto.MonthlyIncome,
                PhoneNumber = dto.PhoneNumber,
                RoleId = roleId,
                PasswordHash = new PasswordHasher().HashPassword(dto.Password),
                
            };
            repository.Add(user);
            unitOfWork.Save();
            return user.Id;
        }
        public void UpdateMonthlyIncome(string email, string password, decimal newIncome)
        {
            var user = repository.FindByEmail(email);
            if (user == null)
            {
                throw new InvalidEmailOrPasswordException();
            }
            var passwordHasher = new PasswordHasher();
            var emailVerificationResult = passwordHasher.VerifyHashedPassword(user.PasswordHash, password);
            if (emailVerificationResult != PasswordVerificationResult.Success)
            {
                throw new InvalidEmailOrPasswordException();
            }
            if (newIncome == user.MonthlyIncome)
            {
                throw new DuplicateMonthlyIncomeException();
            }

            user.MonthlyIncome = newIncome;
            user.VerificationStatus = VerificationStatus.Unverified;
            repository.Update(user);
            unitOfWork.Save();
        }

        public void UpdateJobType(string email, string password, JobType newJobType)
        {
            var user = repository.FindByEmail(email);
            if (user == null)
            {
                throw new InvalidEmailOrPasswordException();
            }
            var passwordHasher = new PasswordHasher();
            var emailVerificationResult = passwordHasher.VerifyHashedPassword(user.PasswordHash, password);
            if (emailVerificationResult != PasswordVerificationResult.Success)
            {
                throw new InvalidEmailOrPasswordException();
            }
            if (newJobType == user.JobType)
            {
                throw new DuplicateJobTypeException();
            }

            user.JobType = newJobType;
            user.VerificationStatus = VerificationStatus.Unverified;
            repository.Update(user);
            unitOfWork.Save();
        }

        public void UpdateFinancialAssets(string email, string password, decimal newAssets)
        {
            var user = repository.FindByEmail(email);
            if (user == null)
            {
                throw new InvalidEmailOrPasswordException();
            }
            var passwordHasher = new PasswordHasher();
            var emailVerificationResult = passwordHasher.VerifyHashedPassword(user.PasswordHash, password);
            if (emailVerificationResult != PasswordVerificationResult.Success)
            {
                throw new InvalidEmailOrPasswordException();
            }
            if (newAssets == user.FinancialAssets)
            {
                throw new DuplicateFinancialAssetsException();
            }

            user.FinancialAssets = newAssets;
            user.VerificationStatus = VerificationStatus.Unverified;
            repository.Update(user);
            unitOfWork.Save();
        }

        
        #endregion
    }
}
