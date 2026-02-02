using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voyage.Migrations
{
    /// <inheritdoc />
    public partial class Update_TeamUserRole_Ak_TeamKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_TeamUserRoles_TeamId_CompanyId_EmployeeId",
                table: "TeamUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_TeamUserRoles_TeamKey",
                table: "TeamUserRoles");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TeamUserRoles_TeamKey_CompanyId_EmployeeId",
                table: "TeamUserRoles",
                columns: new[] { "TeamKey", "CompanyId", "EmployeeId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_TeamUserRoles_TeamKey_CompanyId_EmployeeId",
                table: "TeamUserRoles");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TeamUserRoles_TeamId_CompanyId_EmployeeId",
                table: "TeamUserRoles",
                columns: new[] { "TeamId", "CompanyId", "EmployeeId" });

            migrationBuilder.CreateIndex(
                name: "IX_TeamUserRoles_TeamKey",
                table: "TeamUserRoles",
                column: "TeamKey");
        }
    }
}
