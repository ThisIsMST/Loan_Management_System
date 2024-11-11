using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using loanManagement.Services.Users.Contracts.DTOs;

namespace loanManagement.Services.Users.Contracts.Interfaces
{
    public interface UserQuery
    {
        List<CustomerDto> GetAllCustomers();
        CustomerFinancialDto GetCustomerFinancialData(int customerId);
    }
}
