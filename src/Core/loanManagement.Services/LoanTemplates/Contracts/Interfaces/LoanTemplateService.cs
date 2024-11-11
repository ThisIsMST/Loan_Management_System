using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using loanManagement.Services.LoanTemplates.Contracts.DTOs;

namespace loanManagement.Services.LoanTemplates.Contracts.Interfaces
{
    public interface LoanTemplateService
    {
        int Create(AddLoanTemplateDto dto);
        void Delete(int loanTemplateId);
        GetLoanTemplateDto GetLoanTemplateData(int loanTemplateId);
        void Update(int loanTemplateId, UpdateLoanTemplateDto dto);

        List<LoanTemplateDto> GetAllLoanTemplates();
    }
}
