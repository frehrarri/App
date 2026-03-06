using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voyage.Migrations
{
    /// <inheritdoc />
    public partial class Update_Settings_Sections_RemoveVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Settings_SettingsId_SettingsVersion",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "SettingsVersion",
                table: "Settings");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SprintStartDate",
                table: "Settings",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "SprintStartDate",
                table: "Settings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SettingsVersion",
                table: "Settings",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Settings_SettingsId_SettingsVersion",
                table: "Settings",
                columns: new[] { "SettingsId", "SettingsVersion" });
        }
    }
}
