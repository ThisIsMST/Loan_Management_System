using loanManagement.Services.Installments.Contracts.Interfaces;
using loanManagement.Services.Installments.Exceptions;
using loanManagement.Services.Loans.Contracts.DTOs;
using loanManagement.Services.Loans.Contracts.Interfaces;
using loanManagement.Services.Loans.Exceptions;
using loanManagement.Services.UnitOfWorks;
using LoanManagement.Entities.Installments;
using LoanManagement.Entities.Loans;

namespace loanManagement.Services.Installments
{
    public class InstallmentAppService(
        InstallmentRepository repository,
        LoanRepository loanRepository,
        UnitOfWork unitOfWork,
        InstallmentQuery installmentQuery
        ) : InstallmentService
    {
        public void ScheduleLoanInstallments(RequestedLoanDto requestedLoan)
        {
            var dueDay = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1));
            var installments = new List<Installment>();
            for (int i = 0; i < requestedLoan.DurationMonths; i++)
            {
                installments.Add(new Installment
                {
                    LoanId = requestedLoan.LoanId,
                    PaymentAmount = requestedLoan.MonthlyPayment,
                    DueDate = dueDay,
                    InstallmentStatus = InstallmentStatus.Unpaid,
                });
                dueDay = dueDay.AddMonths(1);
            }
            repository.ScheduleLoanInstallments(installments);
            unitOfWork.Save();
        }

        public void PayLoanInstallment(int loanId)
        {
            var isLoanExist = loanRepository.IsExistById(loanId);
            if (!isLoanExist)
            {
                throw new LoanNotFoundException();
            }
            var loan = loanRepository.Find(loanId);
            bool isThisInstallmentIsTheLastOne = false;
            int remainingInstallment = installmentQuery.GetRemainingInstallmentByLoanId(loan!.Id);
            if (remainingInstallment == 0)
            {
                throw new AllInstallmentsPaidException();
            }
            if (remainingInstallment == 1)
            {
                isThisInstallmentIsTheLastOne = true;
            }
            var FirstUnpaidInstallment = repository.FindFirstUnpaidInstallment(loanId);

            var paymentDay = DateOnly.FromDateTime(DateTime.UtcNow);

            if (paymentDay > FirstUnpaidInstallment.DueDate
                && remainingInstallment > 1)
            {
                FirstUnpaidInstallment.InstallmentStatus = InstallmentStatus.Overdue;
                FirstUnpaidInstallment.InstallmentFine = FirstUnpaidInstallment.PaymentAmount * 0.02m;
                FirstUnpaidInstallment.PaymentAmount += FirstUnpaidInstallment.InstallmentFine;

                repository.PayInstallment(FirstUnpaidInstallment);
                loan.LoanStatus = LoanStatus.Overdue;
                loanRepository.Update(loan);
            }
            else if (remainingInstallment > 1)
            {
                FirstUnpaidInstallment.InstallmentStatus = InstallmentStatus.Paid;
                repository.PayInstallment(FirstUnpaidInstallment);
                loan.LoanStatus = LoanStatus.Repaying;
                loanRepository.Update(loan);
            }
            else if (isThisInstallmentIsTheLastOne)
            {
                FirstUnpaidInstallment.InstallmentStatus = InstallmentStatus.Paid;
                repository.PayInstallment(FirstUnpaidInstallment);
                loan.LoanStatus = LoanStatus.Closed;
                loanRepository.Update(loan);
            }
            unitOfWork.Save();
        }

    }
}
