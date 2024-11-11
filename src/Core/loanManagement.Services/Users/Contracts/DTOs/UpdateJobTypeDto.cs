using LoanManagement.Entities.Users;

namespace loanManagement.Services.Users.Contracts.DTOs
{
    public class UpdateJobTypeDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public JobType NewJobType { get; set; }
    }
}