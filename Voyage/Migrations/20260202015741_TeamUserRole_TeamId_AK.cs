using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voyage.Migrations
{
    /// <inheritdoc />
    public partial class TeamUserRole_TeamId_AK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamUserRoles_Team_TeamKey",
                table: "TeamUserRoles");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_TeamUserRoles_TeamKey_CompanyId_EmployeeId",
                table: "TeamUserRoles");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Team_TeamId",
                table: "Team");

            migrationBuilder.DropIndex(
                name: "IX_Team_CompanyId",
                table: "Team");

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "TeamUserRoles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "Team",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TeamUserRoles_TeamId_CompanyId_EmployeeId",
                table: "TeamUserRoles",
                columns: new[] { "TeamId", "CompanyId", "EmployeeId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Team_CompanyId_TeamId",
                table: "Team",
                columns: new[] { "CompanyId", "TeamId" });

            migrationBuilder.CreateIndex(
                name: "IX_TeamUserRoles_CompanyId_TeamId",
                table: "TeamUserRoles",
                columns: new[] { "CompanyId", "TeamId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TeamUserRoles_Team_CompanyId_TeamId",
                table: "TeamUserRoles",
                columns: new[] { "CompanyId", "TeamId" },
                principalTable: "Team",
                principalColumns: new[] { "CompanyId", "TeamId" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamUserRoles_Team_CompanyId_TeamId",
                table: "TeamUserRoles");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_TeamUserRoles_TeamId_CompanyId_EmployeeId",
                table: "TeamUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_TeamUserRoles_CompanyId_TeamId",
                table: "TeamUserRoles");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Team_CompanyId_TeamId",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "TeamUserRoles");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "Team",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TeamUserRoles_TeamKey_CompanyId_EmployeeId",
                table: "TeamUserRoles",
                columns: new[] { "TeamKey", "CompanyId", "EmployeeId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Team_TeamId",
                table: "Team",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Team_CompanyId",
                table: "Team",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamUserRoles_Team_TeamKey",
                table: "TeamUserRoles",
                column: "TeamKey",
                principalTable: "Team",
                principalColumn: "TeamKey",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
