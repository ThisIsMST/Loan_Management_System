using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoanManagement.Entities.LoanTemplates;

namespace loanManagement.Services.LoanTemplates.Contracts.Interfaces
{
    public interface LoanTemplateRepository
    {
        void Add(LoanTemplate loanTemplate);
        void Delete(LoanTemplate loanTemplate);
        LoanTemplate? Find(int id);
        void Update(LoanTemplate loanTemplate);
    }
}
