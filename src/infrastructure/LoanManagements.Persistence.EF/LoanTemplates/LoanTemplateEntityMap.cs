using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoanManagement.Entities.LoanTemplates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanManagement.Persistence.EF.LoanTemplates
{
    public class LoanTemplateEntityMap : IEntityTypeConfiguration<LoanTemplate>
    {
        public void Configure(EntityTypeBuilder<LoanTemplate> builder)
        {
            builder.ToTable("LoanTemplates").HasKey(_ => _.Id);
            builder.Property(_ => _.Id).ValueGeneratedOnAdd();
            builder.Property(_ => _.LoanAmount).IsRequired();
            builder.Property(_ => _.AnnualInterestRate).IsRequired();
            builder.Property(_ => _.DurationMonths).IsRequired();
        }
    }
}
