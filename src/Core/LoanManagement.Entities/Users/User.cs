
namespace LoanManagement.Entities.Users
{
    public class User
    {
        public  int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string NationalId { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required string PhoneNumber { get; set; }
        public decimal? MonthlyIncome { get; set; }
        public JobType? JobType { get; set; }
        public decimal? FinancialAssets { get; set; }
        public VerificationStatus? VerificationStatus { get; set; } 
        public int RoleId { get; set; }
        public int? CustomerScore { get; set; }
    }
    public enum JobType
    {
        Employee = 0,
        SelfEmployed,
        Unemployed,
    }
    public enum VerificationStatus
    {
        Unverified , 
        Requested ,
        Verified ,
        Failed 
    }
}