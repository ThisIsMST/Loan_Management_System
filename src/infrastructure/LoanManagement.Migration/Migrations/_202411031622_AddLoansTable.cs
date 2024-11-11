using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace LoanManagement.Migrations.Migrations
{
    [Migration(202411031622)]
    public class _202411031622_AddLoansTable : Migration
    {
        public override void Up()
        {
            Create.Table("Loans")
                 .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                 .WithColumn("UserId").AsInt32().NotNullable()
                 .WithColumn("LoanAmount").AsDecimal().NotNullable()
                 .WithColumn("AnnualInterestRate").AsInt32().NotNullable()
                 .WithColumn("DurationMonths").AsInt32().NotNullable()
                 .WithColumn("StartDate").AsDate().Nullable()
                 .WithColumn("LoanStatus").AsInt32().NotNullable();

        }
        public override void Down()
        {
            Delete.Table("Loans");
        }
    }
}
