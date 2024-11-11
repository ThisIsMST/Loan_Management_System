using LoanManagement.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanManagement.Persistence.EF.Users
{
    public class UserEntityMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(ـ => ـ.Id);
            builder.Property(ـ => ـ.FirstName).IsRequired().HasMaxLength(255);
            builder.Property(ـ => ـ.LastName).IsRequired().HasMaxLength(255);
            builder.Property(ـ => ـ.NationalId).IsRequired().HasMaxLength(50);
            builder.Property(ـ => ـ.Email).IsRequired().HasMaxLength(255);
            builder.Property(ـ => ـ.PasswordHash).IsRequired().HasMaxLength(500);
            builder.Property(ـ => ـ.PhoneNumber).IsRequired().HasMaxLength(50);
            builder.Property(ـ => ـ.MonthlyIncome).IsRequired(false);
            builder.Property(ـ => ـ.JobType).IsRequired(false).HasDefaultValue(JobType.Unemployed);
            builder.Property(ـ => ـ.FinancialAssets).IsRequired(false);
            builder.Property(ـ => ـ.VerificationStatus).IsRequired(false);
            builder.Property(ـ => ـ.RoleId).IsRequired();
            builder.Property(ـ => ـ.CustomerScore).IsRequired(false).HasDefaultValue(1);
        }
    }
}
