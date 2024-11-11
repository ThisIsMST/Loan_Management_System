using System.Xml;
using loanManagement.Services.LoanTemplates.Contracts.DTOs;
using loanManagement.Services.LoanTemplates.Contracts.Interfaces;
using loanManagement.Services.LoanTemplates.Exceptions;
using loanManagement.Services.UnitOfWorks;
using LoanManagement.Entities.LoanTemplates;

namespace loanManagement.Services.LoanTemplates
{
    public class LoanTemplateAppService(
        LoanTemplateRepository repository
        ,LoanTemplateQuery loanTemplateQuery,
        UnitOfWork unitOfWork) : LoanTemplateService
    {
        public GetLoanTemplateDto GetLoanTemplateData(int loanTemplateId)
        {
            return loanTemplateQuery.GetLoanTemplateData(loanTemplateId);
        }
        public int Create(AddLoanTemplateDto dto) 
        {
            
            if (dto.AnnualInterestRate > 20 || dto.AnnualInterestRate < 15)
            {
                throw new InvalidAnnualInterestRateException();
            }
            if(dto.LoanAmount < 0)
            {
                throw new InvalidLoanAmountException();
            }
            if (dto.DurationMonths < 0) 
            {
                throw new InvalidDurationMonthsException();
            }
            var loanTemplate = new LoanTemplate
            {
                LoanAmount = dto.LoanAmount,
                AnnualInterestRate = dto.AnnualInterestRate,
                DurationMonths = dto.DurationMonths
            };

            repository.Add(loanTemplate);
            unitOfWork.Save();
            return loanTemplate.Id;
        }
        public void Update(int loanTemplateId , UpdateLoanTemplateDto dto) 
        {
            var loanTemplate = repository.Find(loanTemplateId);
            if (loanTemplate == null)
            {
                throw new LoanTemplateNotFoundException();
            }
            if (dto.AnnualInterestRate > 20 || dto.AnnualInterestRate < 15)
            {
                throw new InvalidAnnualInterestRateException();
            }
            if (dto.LoanAmount < 0)
            {
                throw new InvalidLoanAmountException();
            }
            if (dto.DurationMonths < 0)
            {
                throw new InvalidDurationMonthsException();
            }
            loanTemplate.AnnualInterestRate = dto.AnnualInterestRate;
            loanTemplate.LoanAmount = dto.LoanAmount;
            loanTemplate.DurationMonths = dto.DurationMonths;
            repository.Update(loanTemplate);
            unitOfWork.Save();
        }
        public void Delete(int loanTemplateId)
        {
            var loanTemplate = repository.Find(loanTemplateId);
            if (loanTemplate == null) 
            {
                throw new LoanTemplateNotFoundException();
            }
            repository.Delete(loanTemplate);
            unitOfWork.Save();
        }

        public List<LoanTemplateDto> GetAllLoanTemplates()
        {
            return loanTemplateQuery.GetAllLoanTemplates();
        }
    }
}
