using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BankingSystem.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.EnsureSchema(
                name: "branch1");

            migrationBuilder.EnsureSchema(
                name: "public");
                */

            migrationBuilder.CreateTable(
                name: "tenants",
                schema: "common",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    encrypted_tenant_key = table.Column<byte[]>(type: "bytea", nullable: false),
                    tenant_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    connection_string =
                        table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table => { table.PrimaryKey("tenant_pkey", x => x.tenant_id); });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "tenants",
                schema: "common");
       }
    }
}
