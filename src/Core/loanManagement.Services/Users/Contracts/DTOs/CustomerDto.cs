using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoanManagement.Entities.Users;

namespace loanManagement.Services.Users.Contracts.DTOs
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string NationalId { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public decimal MonthlyIncome { get; set; }
        public JobType CustomerJobType { get; set; }
        public decimal FinancialAssets { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
    }
}
