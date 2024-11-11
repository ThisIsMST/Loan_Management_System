using FluentAssertions;
using loanManagement.Services.LoanTemplates.Contracts.DTOs;
using loanManagement.Services.LoanTemplates.Contracts.Interfaces;
using LoanManagement.Entities.LoanTemplates;
using LoanManagement.Persistence.EF.LoanTemplates;
using LoanManagement.TestTools.Infrastructure.DataBaseConfig.Integration;
using LoanManagement.TestTools.LoanTemplates;

namespace LoanManagements.Service.Unit.Tests.LoanTemplates
{
    public class LoanTemplateQueryTests : BusinessIntegrationTest
    {
        private readonly LoanTemplateQuery _sut;
        public LoanTemplateQueryTests()
        {
            _sut = new EFLoanTemplateQuery(SetupContext);
        }
        [Fact]
        public void GetLoanTemplateData_get_loan_template_data_properly()
        {
            var loanTemplate = new LoanTemplateBuilder()
                .WithAnnualInterestRate(20)
                .WithLoanAmount(100000)
                .WithDurationMonths(24)
                .Build();
            Save(loanTemplate);

            var actual = ReadContext.Set<LoanTemplate>().SingleOrDefault(_=>_.Id == loanTemplate.Id);

            actual.Should().NotBeNull();
            actual!.DurationMonths.Should().Be(24);
            actual.AnnualInterestRate.Should().Be(20);
            actual.LoanAmount.Should().Be(100000);


        }
        [Fact]
        public void GetAllLoanTemplates_get_all_loan_templates_properly()
        {
            var template = new LoanTemplateBuilder().Build();
            Save(template);


            var actual = _sut.GetAllLoanTemplates();
            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(new LoanTemplateDto
            {
                Id = template.Id,
                DurationMonths = template.DurationMonths,
                AnnualInterestRate = template.AnnualInterestRate,
                InstallmentCount = template.InstallmentCount,
                LoanAmount = template.LoanAmount,
            });
        }


    }
}
