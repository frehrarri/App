using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voyage.Migrations
{
    /// <inheritdoc />
    public partial class Update_TicketSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SprintEndDate",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "SprintEndDate",
                table: "Settings");

            migrationBuilder.AddColumn<int>(
                name: "SprintLength",
                table: "Tickets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SprintLength",
                table: "Settings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SprintLength",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "SprintLength",
                table: "Settings");

            migrationBuilder.AddColumn<DateTime>(
                name: "SprintEndDate",
                table: "Tickets",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SprintEndDate",
                table: "Settings",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
