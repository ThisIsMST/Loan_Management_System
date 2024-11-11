using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoanManagement.Entities.Users;

namespace loanManagement.Services.Users.Contracts.DTOs
{
    public class CustomerSignUpDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string NationalId { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string PhoneNumber { get; set; }
        public decimal MonthlyIncome { get; set; }
        public JobType JobType { get; set; }
        public decimal FinancialAssets { get; set; }
        public VerificationStatus VerificationStatus  = VerificationStatus.Unverified;
        public int RoleId { get; set; }
        public int CustomerScore = 0;
    }
}
