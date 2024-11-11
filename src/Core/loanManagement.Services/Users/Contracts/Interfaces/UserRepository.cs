using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoanManagement.Entities.Users;

namespace loanManagement.Services.Users.Contracts.Interfaces
{
    public interface UserRepository
    {
        void Add(User user);
        void AddLoanToCustomerAssets(int customerId, decimal loanAmount);
        void Delete(User user);
        User? FindByEmail(string email);
        User? FindById(int id);
        bool IsEmailDuplicate(string email);
        bool IsExistById(int id);
        void Update(User user);
    }
}
