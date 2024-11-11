using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using loanManagement.Services.Roles.Contracts.Interfaces;
using LoanManagement.Entities.Roles;
using LoanManagement.Entities.Users;
using LoanManagement.Persistence.EF.DataContext;

namespace LoanManagement.Persistence.EF.Roles
{
    public class EFRoleRepository(EFDataContext context) : RoleRepository
    {
        public int AddAdmin()
        {
            var adminRole = new Role
            {
                RoleName = "admin",
            };
            context.Set<Role>().Add(adminRole);
            context.SaveChanges();
            return adminRole.Id;
        }

        public int AddCustomer()
        {
            var customerRole = new Role
            {
                RoleName = "customer",
            };
            context.Set<Role>().Add(customerRole);
            context.SaveChanges();
            return customerRole.Id;
        }

        public string GetRoleByEmail(string Email)
        {
            return
                (
                from user in context.Set<User>()
                where user.Email == Email
                join role in context.Set<Role>()
                on user.RoleId equals role.Id
                select role.RoleName
                ).Single();
        }
        public bool IsUserCustomer(int roleId) 
        {
           if (context.Set<Role>().First(_=>_.Id == roleId).RoleName == "customer")
            {
                return true;
            }
           else return false;
        }
    }
}
