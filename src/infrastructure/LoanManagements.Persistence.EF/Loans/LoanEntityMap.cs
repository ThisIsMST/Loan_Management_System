using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoanManagement.Entities.Loans;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Persistence.EF.Loans
{
    public class LoanEntityMap : IEntityTypeConfiguration<Loan>
    {
        public void Configure(EntityTypeBuilder<Loan> builder)
        {
            builder.ToTable("Loans");
            builder.HasKey(_ => _.Id);
            builder.Property(_ => _.Id).ValueGeneratedOnAdd();
            builder.Property(_ => _.LoanAmount).IsRequired();
            builder.Property(_ => _.AnnualInterestRate).IsRequired();
            builder.Property(_ => _.DurationMonths).IsRequired();
            builder.Property(_ => _.StartDate).IsRequired(false);
            builder.Property(_ => _.LoanStatus).IsRequired();       
        }
    }
}

