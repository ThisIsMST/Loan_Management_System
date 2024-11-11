using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using loanManagement.Services.LoanTemplates.Contracts.DTOs;
using loanManagement.Services.LoanTemplates.Contracts.Interfaces;
using LoanManagement.Entities.LoanTemplates;
using LoanManagement.Persistence.EF.DataContext;

namespace LoanManagement.Persistence.EF.LoanTemplates
{
    public class EFLoanTemplateQuery(EFDataContext context) : LoanTemplateQuery
    {
        public List<LoanTemplateDto> GetAllLoanTemplates()
        {
            return (
                from lt in context.Set<LoanTemplate>()
                select new LoanTemplateDto
                {
                    Id = lt.Id,
                    DurationMonths = lt.DurationMonths,
                    AnnualInterestRate = lt.AnnualInterestRate,
                    LoanAmount = lt.LoanAmount,
                    InstallmentCount = lt.InstallmentCount,
                }
                ).ToList();
        }

        public GetLoanTemplateDto GetLoanTemplateData(int loanTemplateId)
        {
            return (
                from lt in context.Set<LoanTemplate>()
                where lt.Id == loanTemplateId
                select new GetLoanTemplateDto
                {
                    LoanAmount = lt.LoanAmount,
                    AnnualInterestRate = lt.AnnualInterestRate,
                    DurationMonths = lt.DurationMonths,
                    InstallmentCount = lt.InstallmentCount,
                }
                ).Single();
        }
    }
}
