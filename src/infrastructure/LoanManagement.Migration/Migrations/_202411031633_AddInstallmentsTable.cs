using FluentMigrator;

namespace LoanManagement.Migrations.Migrations
{
    [Migration(202411031633)]
    public class _202411031633_AddInstallmentsTable : Migration
    {

        public override void Up()
        {
            Create.Table("Installments")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("LoanId").AsInt32().NotNullable()
                .WithColumn("PaymentAmount").AsDecimal().NotNullable()
                .WithColumn("InstallmentFine").AsDecimal().NotNullable().WithDefaultValue(0)
                .WithColumn("PaymentDate").AsDate().Nullable()
                .WithColumn("DueDate").AsDate().NotNullable()
                .WithColumn("installmentStatus").AsInt32().WithDefaultValue(0);
        }
        public override void Down()
        {
            Delete.Table("Installments");
        }
    }
}
