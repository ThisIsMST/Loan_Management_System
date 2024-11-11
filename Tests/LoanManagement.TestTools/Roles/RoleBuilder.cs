using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using loanManagement.Services.Roles.Exceptions;
using LoanManagement.Entities.Roles;
using Microsoft.IdentityModel.Tokens;

namespace LoanManagement.TestTools.Roles
{
    
    public class RoleBuilder
    {
        private readonly Role _role;

        public RoleBuilder()
        {
            _role = new Role();
        }


        public RoleBuilder WithCustomerRole()
        {
            _role.RoleName = "customer";
            return this;
        }
        public RoleBuilder WithAdminRole()
        {
            _role.RoleName = "admin";
            return this;
        }
        public Role Build(){
            if (_role.RoleName.IsNullOrEmpty()) 
            {
                throw new RoleNotSpecifiedException();
            }
            return _role;

        }
    }
}
