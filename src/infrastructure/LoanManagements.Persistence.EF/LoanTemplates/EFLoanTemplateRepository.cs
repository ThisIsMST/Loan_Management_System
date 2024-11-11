using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using loanManagement.Services.LoanTemplates.Contracts.Interfaces;
using LoanManagement.Entities.LoanTemplates;
using LoanManagement.Persistence.EF.DataContext;

namespace LoanManagement.Persistence.EF.LoanTemplates
{
    public class EFLoanTemplateRepository(EFDataContext context) : LoanTemplateRepository
    {
        public void Add(LoanTemplate loanTemplate)
        {
            context.Set<LoanTemplate>().Add(loanTemplate);
        }
        public void Update(LoanTemplate loanTemplate) 
        {
            context.Set<LoanTemplate>().Update(loanTemplate);
        }
        public void Delete(LoanTemplate loanTemplate)
        {
            context.Set<LoanTemplate>().Remove(loanTemplate);
        }
        public LoanTemplate? Find(int id)
        {
            return context.Set<LoanTemplate>().FirstOrDefault(_ => _.Id == id);
        }
    }
}
