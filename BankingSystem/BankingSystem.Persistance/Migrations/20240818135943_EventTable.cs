using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankingSystem.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class EventTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "event_pkey",
                table: "events");

            migrationBuilder.DropColumn(
                name: "id",
                table: "events");

            migrationBuilder.DropColumn(
                name: "eventdate",
                table: "events");

            migrationBuilder.RenameColumn(
                name: "eventtype",
                table: "events",
                newName: "event_type");

            migrationBuilder.RenameColumn(
                name: "eventdata",
                table: "events",
                newName: "event_data");

            migrationBuilder.AddColumn<Guid>(
                name: "event_id",
                table: "events",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()");

            migrationBuilder.AddColumn<Guid>(
                name: "aggregate_id",
                table: "events",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "events",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_events",
                table: "events",
                column: "event_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_events",
                table: "events");

            migrationBuilder.DropColumn(
                name: "event_id",
                table: "events");

            migrationBuilder.DropColumn(
                name: "aggregate_id",
                table: "events");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "events");

            migrationBuilder.RenameColumn(
                name: "event_type",
                table: "events",
                newName: "eventtype");

            migrationBuilder.RenameColumn(
                name: "event_data",
                table: "events",
                newName: "eventdata");

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "events",
                type: "integer",
                nullable: false,
                defaultValueSql: "nextval('event_id_seq'::regclass)");

            migrationBuilder.AddColumn<DateTime>(
                name: "eventdate",
                table: "events",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddPrimaryKey(
                name: "event_pkey",
                table: "events",
                column: "id");
        }
    }
}
