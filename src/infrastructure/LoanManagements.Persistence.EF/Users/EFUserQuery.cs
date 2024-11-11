using loanManagement.Services.Users.Contracts.DTOs;
using loanManagement.Services.Users.Contracts.Interfaces;
using LoanManagement.Entities.Roles;
using LoanManagement.Entities.Users;
using LoanManagement.Persistence.EF.DataContext;

namespace LoanManagement.Persistence.EF.Users
{
    public class EFUserQuery(EFDataContext context) : UserQuery
    {
        public List<CustomerDto> GetAllCustomers()
        {
            return (
                from c in context.Set<User>()
                join r in context.Set<Role>()
                on c.RoleId equals r.Id
                where r.RoleName == "customer"
                select new CustomerDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    NationalId = c.NationalId,
                    PhoneNumber = c.PhoneNumber,
                    VerificationStatus = (VerificationStatus)c.VerificationStatus,
                    FinancialAssets = (decimal)c.FinancialAssets,
                    CustomerJobType = (JobType)c.JobType ,
                    MonthlyIncome = (decimal)c.MonthlyIncome,  
                }).ToList();
        }

        public CustomerFinancialDto GetCustomerFinancialData(int customerId)
        {
            return (
                from u in context.Set<User>()
                where u.Id == customerId
                select new CustomerFinancialDto
                {
                    FinancialAssets = (decimal)u.FinancialAssets,
                    JobType = (JobType)u.JobType,
                    MonthlyIncome = (decimal)u.MonthlyIncome,
                }
                ).Single();
        }
    }
}
