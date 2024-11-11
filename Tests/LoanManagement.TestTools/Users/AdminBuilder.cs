using LoanManagement.Entities.Users;
using Microsoft.AspNet.Identity;

namespace LoanManagement.TestTools.Users
{
    public class AdminBuilder
    {
        private readonly User _admin;

        public AdminBuilder(int roleId)
        {
            _admin = new User
            {
                RoleId = roleId,
                FirstName = "Test",
                LastName = "Test",
                NationalId = "1234567890",
                Email = "test@test.com",
                PasswordHash = new PasswordHasher().HashPassword("Test@123"),
                PhoneNumber = "0123456789",
                
            };
        }
        public AdminBuilder WithFirstName(string firstName)
        {
            _admin.FirstName = firstName;
            return this;
        }
        public AdminBuilder WithLastName(string lastName)
        {
            _admin.LastName = lastName;
            return this;
        }
        public AdminBuilder WithNationalId(string nationalId)
        {
            _admin.NationalId = nationalId;
            return this;
        }
        public AdminBuilder WithEmail(string email)
        {
            _admin.Email = email;
            return this;
        }
        public AdminBuilder WithPhoneNumber(string phoneNumber)
        {
            _admin.PhoneNumber = phoneNumber;
            return this;
        }
        public AdminBuilder WithPassword(string password)
        {
            _admin.PasswordHash = new PasswordHasher().HashPassword(password);
            return this;
        }
        public User Build() => _admin;
    }
}
