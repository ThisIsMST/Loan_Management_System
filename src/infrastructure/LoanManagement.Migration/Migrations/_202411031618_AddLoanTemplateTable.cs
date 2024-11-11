using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace LoanManagement.Migrations.Migrations
{
    [Migration(202411031618)]
    public class _202411031618_AddLoanTemplateTable : Migration
    {
        public override void Up()
        {
            Create.Table("LoanTemplates")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("LoanAmount").AsDecimal().NotNullable()
                .WithColumn("AnnualInterestRate").AsInt32().NotNullable()
                .WithColumn("DurationMonths").AsInt32().NotNullable();
        }

        public override void Down()
        {
            Delete.Table("LoanTemplates");
        }
    }
}
