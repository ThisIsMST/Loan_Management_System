using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using LoanManagement.Entities.LoanTemplates;
using LoanManagement.Persistence.EF.DataContext;
using LoanManagement.Persistence.EF.LoanTemplates;
using loanManagement.Services.LoanTemplates.Contracts.DTOs;
using loanManagement.Services.LoanTemplates.Exceptions;
using loanManagement.Services.LoanTemplates;
using LoanManagement.TestTools.Infrastructure.DataBaseConfig.Integration;
using LoanManagement.TestTools.LoanTemplates;

namespace LoanManagements.Service.Unit.Tests.LoanTemplates
{
    public class LoanTemplateAppServiceTests : BusinessIntegrationTest
    {
        private readonly LoanTemplateAppService _sut;

        public LoanTemplateAppServiceTests()
        {
            var repository = new EFLoanTemplateRepository(SetupContext);
            var loanTemplateQuery = new EFLoanTemplateQuery(SetupContext);
            var unitOfWork = new EFUnitOfWork(SetupContext);
            _sut = new LoanTemplateAppService(repository, loanTemplateQuery, unitOfWork);
        }

        [Fact]
        public void Create_create_loan_template_properly()
        {
            var dto = new AddLoanTemplateDto
            {
                LoanAmount = 50000,
                AnnualInterestRate = 18,
                DurationMonths = 12
            };

            var loanTemplateId = _sut.Create(dto);

            var actual = ReadContext.Set<LoanTemplate>().FirstOrDefault(l => l.Id == loanTemplateId);
            actual.Should().NotBeNull();
            actual!.LoanAmount.Should().Be(50000);
            actual.AnnualInterestRate.Should().Be(18);
            actual.DurationMonths.Should().Be(12);
        }

        [Fact]
        public void Create_throw_exception_when_interest_rate_is_out_of_range()
        {
            var dto = new AddLoanTemplateDto
            {
                AnnualInterestRate = 25, 
                LoanAmount = 50000,          
                DurationMonths = 12
            };

            var exception = () => _sut.Create(dto);

            exception.Should().ThrowExactly<InvalidAnnualInterestRateException>();
        }

        [Fact]
        public void Create_throw_exception_when_loan_amount_is_invalid()
        {
            var dto = new AddLoanTemplateDto
            {
                LoanAmount = -1000, 
                AnnualInterestRate = 18,
                DurationMonths = 12
            };

            var exception = () => _sut.Create(dto);

            exception.Should().ThrowExactly<InvalidLoanAmountException>();
        }

        [Fact]
        public void Create_throw_exception_when_duration_is_invalid()
        {
            var dto = new AddLoanTemplateDto
            {
                DurationMonths = -6 ,
                LoanAmount = 50000,
                AnnualInterestRate = 18
            };

            var exception = () => _sut.Create(dto);

            exception.Should().ThrowExactly<InvalidDurationMonthsException>();
        }

        [Fact]
        public void Update_update_loan_template_properly()
        {
            var loanTemplate = new LoanTemplateBuilder()
                .WithLoanAmount(50000)
                .WithAnnualInterestRate(18)
                .WithDurationMonths(12)
                .Build();
            Save(loanTemplate);
            var dto = new UpdateLoanTemplateDto
            {
                LoanAmount = 60000,
                AnnualInterestRate = 20,
                DurationMonths = 24
            };

            _sut.Update(loanTemplate.Id, dto);

            var actual = ReadContext.Set<LoanTemplate>().First(l => l.Id == loanTemplate.Id);
            actual.LoanAmount.Should().Be(60000);
            actual.AnnualInterestRate.Should().Be(20);
            actual.DurationMonths.Should().Be(24);
        }

        [Fact]
        public void Update_ShouldThrowLoanTemplateNotFoundException_WhenTemplateDoesNotExist()
        {
            var nonExistentId = 999;
            var dto = new UpdateLoanTemplateDto
            {
                LoanAmount = 50000,
                AnnualInterestRate = 18,
                DurationMonths = 12
            };

            var exception = () => _sut.Update(nonExistentId, dto);

            exception.Should().ThrowExactly<LoanTemplateNotFoundException>();
        }

        [Fact]
        public void Update_ShouldThrowInvalidAnnualInterestRateException_WhenInterestRateIsOutOfRange()
        {
            var loanTemplate = new LoanTemplateBuilder()
                .WithLoanAmount(50000)
                .WithAnnualInterestRate(18)
                .WithDurationMonths(12)
                .Build();
            Save(loanTemplate);
            var dto = new UpdateLoanTemplateDto
            {
                LoanAmount = 60000,
                AnnualInterestRate = 25, // Invalid rate
                DurationMonths = 24
            };

            var exception = () => _sut.Update(loanTemplate.Id, dto);

            exception.Should().ThrowExactly<InvalidAnnualInterestRateException>();
        }

        [Fact]
        public void Delete_ShouldRemoveLoanTemplate_WhenTemplateExists()
        {
            var loanTemplate = new LoanTemplateBuilder()
                .WithLoanAmount(50000)
                .WithAnnualInterestRate(18)
                .WithDurationMonths(12)
                .Build();
            Save(loanTemplate);

            _sut.Delete(loanTemplate.Id);

            var actual = ReadContext.Set<LoanTemplate>().FirstOrDefault(l => l.Id == loanTemplate.Id);
            actual.Should().BeNull();
        }

        [Fact]
        public void Delete_ShouldThrowLoanTemplateNotFoundException_WhenTemplateDoesNotExist()
        {
            var nonExistentId = 999;

            var exception = () => _sut.Delete(nonExistentId);

            exception.Should().ThrowExactly<LoanTemplateNotFoundException>();
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

