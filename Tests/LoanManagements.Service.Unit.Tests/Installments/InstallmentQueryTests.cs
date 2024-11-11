using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using loanManagement.Services.Installments.Contracts.Interfaces;
using LoanManagement.Entities.Installments;
using LoanManagement.Entities.Loans;
using LoanManagement.Persistence.EF.Installments;
using LoanManagement.TestTools.Infrastructure.DataBaseConfig.Integration;
using LoanManagement.TestTools.Installments;
using LoanManagement.TestTools.Loans;

namespace LoanManagements.Service.Unit.Tests.Installments
{
    public class InstallmentQueryTests : BusinessIntegrationTest
    {
        private readonly InstallmentQuery _sut;

        public InstallmentQueryTests()
        {
            _sut = new EFInstallmentQuery(SetupContext);
        }

        [Theory]
        [InlineData(1)]
        public void GetRemainingInstallmentByLoanId_get_remaining_installments_properly(int fakeCustomerId)
        {
            var loan = new LoanBuilder(fakeCustomerId).WithLoanStatus(LoanStatus.Repaying).Build();
            Save(loan);
            var installment1 = new InstallmentBuilder(loan.Id).WithInstallmentStatus(InstallmentStatus.Paid).Build();
            var installment2 = new InstallmentBuilder(loan.Id).WithInstallmentStatus(InstallmentStatus.Unpaid).Build();
            Save(installment1);
            Save(installment2);

            var actual = _sut.GetRemainingInstallmentByLoanId(loan.Id);

            actual.Should().Be(1);
        }

    }
}
