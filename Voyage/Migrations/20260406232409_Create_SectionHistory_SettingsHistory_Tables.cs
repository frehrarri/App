using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voyage.Migrations
{
    /// <inheritdoc />
    public partial class Create_SectionHistory_SettingsHistory_Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SettingsVersion",
                table: "Settings",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    LogId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Severity = table.Column<int>(type: "integer", nullable: false),
                    StackTrace = table.Column<string>(type: "text", nullable: true),
                    ClientMessage = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    IsLatest = table.Column<bool>(type: "boolean", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.LogId);
                });

            migrationBuilder.CreateTable(
                name: "SettingsHistory",
                columns: table => new
                {
                    SettingsHistoryKey = table.Column<Guid>(type: "uuid", nullable: false),
                    SettingsVersion = table.Column<decimal>(type: "numeric", nullable: false),
                    Feature = table.Column<int>(type: "integer", nullable: false),
                    RepeatSprintOption = table.Column<int>(type: "integer", nullable: false),
                    SprintStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SprintLength = table.Column<int>(type: "integer", nullable: false),
                    SprintId = table.Column<int>(type: "integer", nullable: false),
                    SectionSetting = table.Column<int>(type: "integer", nullable: false),
                    TeamKey = table.Column<Guid>(type: "uuid", nullable: true),
                    DepartmentKey = table.Column<Guid>(type: "uuid", nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: true),
                    EmployeeId = table.Column<int>(type: "integer", nullable: true),
                    RoleId = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    SettingsId = table.Column<int>(type: "integer", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    IsLatest = table.Column<bool>(type: "boolean", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingsHistory", x => x.SettingsHistoryKey);
                    table.UniqueConstraint("AK_SettingsHistory_SettingsId_SettingsVersion", x => new { x.SettingsId, x.SettingsVersion });
                    table.ForeignKey(
                        name: "FK_SettingsHistory_Settings_SettingsId",
                        column: x => x.SettingsId,
                        principalTable: "Settings",
                        principalColumn: "SettingsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SectionsHistory",
                columns: table => new
                {
                    SectionHistoryKey = table.Column<Guid>(type: "uuid", nullable: false),
                    SectionId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    SectionOrder = table.Column<int>(type: "integer", nullable: false),
                    SettingsHistoryKey = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    IsLatest = table.Column<bool>(type: "boolean", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionsHistory", x => x.SectionHistoryKey);
                    table.ForeignKey(
                        name: "FK_SectionsHistory_SettingsHistory_SettingsHistoryKey",
                        column: x => x.SettingsHistoryKey,
                        principalTable: "SettingsHistory",
                        principalColumn: "SettingsHistoryKey",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SectionsHistory_SettingsHistoryKey",
                table: "SectionsHistory",
                column: "SettingsHistoryKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "SectionsHistory");

            migrationBuilder.DropTable(
                name: "SettingsHistory");

            migrationBuilder.DropColumn(
                name: "SettingsVersion",
                table: "Settings");
        }
    }
}
