using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using loanManagement.Services.LoanTemplates.Contracts.DTOs;

namespace loanManagement.Services.LoanTemplates.Contracts.Interfaces
{
    public interface LoanTemplateQuery
    {
        List<LoanTemplateDto> GetAllLoanTemplates();
        GetLoanTemplateDto GetLoanTemplateData(int loanTemplateId);
    }
}
