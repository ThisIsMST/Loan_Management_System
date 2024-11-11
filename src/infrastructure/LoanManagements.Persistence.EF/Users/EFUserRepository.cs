
using loanManagement.Services.Users.Contracts.Interfaces;
using loanManagement.Services.Users.Exceptions;
using LoanManagement.Entities.Users;
using LoanManagement.Persistence.EF.DataContext;

namespace LoanManagement.Persistence.EF.Users
{
    public class EFUserRepository(EFDataContext context) : UserRepository
    {
        public void Add(User user)
        {
            context.Set<User>().Add(user);
        }

        public void AddLoanToCustomerAssets(int customerId, decimal loanAmount)
        {
            var customer = context.Set<User>().Single(_ => _.Id == customerId);
            customer.FinancialAssets += loanAmount;
            context.Set<User>().Update(customer);
        }

        public void Delete(User user)
        {
            context.Set<User>().Remove(user);
        }

        public User? FindByEmail(string email)
        {
            return context.Set<User>()
                 .SingleOrDefault(user => user.Email.ToLower() == email.ToLower());
        }

        public User? FindById(int id)
        {
            return context.Set<User>().SingleOrDefault(_ => _.Id == id);
        }

        public bool IsEmailDuplicate(string email)
        {
            return context.Set<User>().Any(_ => _.Email == email);

        }

        public bool IsExistById(int id)
        {
            return context.Set<User>().Any(_ => _.Id == id);
        }

        public void Update(User user)
        {
            context.Set<User>().Update(user);
        }
    }
}
