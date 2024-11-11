using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoanManagement.Entities.Installments;
using LoanManagement.Entities.Loans;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Persistence.EF.Installments
{
    public class InstallmentEntityMap : IEntityTypeConfiguration<Installment>
    {
        public void Configure(EntityTypeBuilder<Installment> builder)
        {
            builder.ToTable("Installments");
            builder.HasKey(_ => _.Id);
            builder.Property(_ => _.Id).ValueGeneratedOnAdd();
            builder.Property(_ => _.LoanId).IsRequired();
            builder.Property(_ => _.PaymentAmount).IsRequired();
            builder.Property(_ => _.InstallmentFine).IsRequired().HasDefaultValue(0);
            builder.Property(_ => _.PaymentDate).IsRequired(false);
            builder.Property(_ => _.DueDate).IsRequired();
            builder.Property(_=>_.InstallmentStatus).IsRequired().HasDefaultValue(InstallmentStatus.Unpaid);
        }
    }

}
