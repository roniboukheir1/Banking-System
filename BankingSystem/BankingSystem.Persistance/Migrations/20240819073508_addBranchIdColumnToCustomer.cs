using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankingSystem.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class addBranchIdColumnToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "event_id",
                table: "events",
                type: "integer",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v4()");

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                schema: "branch1",
                table: "customers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchId",
                schema: "branch1",
                table: "customers");

            migrationBuilder.AlterColumn<Guid>(
                name: "event_id",
                table: "events",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()",
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValueSql: "uuid_generate_v4()");
        }
    }
}
