using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace LoanManagement.Migrations.Migrations
{
    [Migration(202411031616)]
    public class _202411031616_AddRolesTable : Migration
    {
        public override void Up()
        {
             Create.Table("Roles")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("RoleName").AsString().NotNullable();
        }
        public override void Down()
        {
            Delete.Table("Roles");
        }

    }
}
