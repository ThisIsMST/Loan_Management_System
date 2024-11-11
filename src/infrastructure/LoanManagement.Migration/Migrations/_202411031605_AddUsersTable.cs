using FluentMigrator;

namespace LoanManagement.Migrations.Migrations
{
    [Migration(202411031607)] 
    public class _202411031607_AddUsersTable : Migration
    {
        public override void Up()
        {
            Create.Table("Users")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("FirstName").AsString(255).NotNullable()
                .WithColumn("LastName").AsString(255).NotNullable()
                .WithColumn("NationalId").AsString(50).NotNullable()
                .WithColumn("Email").AsString(255).NotNullable()
                .WithColumn("PasswordHash").AsString(500).NotNullable()
                .WithColumn("PhoneNumber").AsString(50).NotNullable()
                .WithColumn("MonthlyIncome").AsDecimal().Nullable()
                .WithColumn("JobType").AsInt32().Nullable().WithDefaultValue(2)
                .WithColumn("FinancialAssets").AsDecimal().Nullable()
                .WithColumn("VerificationStatus").AsInt32().Nullable()
                .WithColumn("RoleId").AsInt32().NotNullable()
                .WithColumn("CustomerScore").AsInt32().Nullable().WithDefaultValue(1);
        }
        public override void Down()
        {
            Delete.Table("Users");
        }
    }
}
