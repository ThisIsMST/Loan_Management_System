using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using loanManagement.Services.UnitOfWorks;

namespace LoanManagement.Persistence.EF.DataContext
{
    public class EFUnitOfWork(EFDataContext context) : UnitOfWork
    {
        public void Begin()
        {
            context.Database.BeginTransaction();
        }

        public void Commit()
        {
            context.Database.CommitTransaction();
        }

        public void Rollback()
        {
            context.Database.RollbackTransaction();
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
