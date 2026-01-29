using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voyage.Migrations
{
    /// <inheritdoc />
    public partial class update_CompanyRole_AltKey_RemovedVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_CompanyRoles_CompanyId_RoleId_RoleVersion",
                table: "CompanyRoles");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_CompanyRoles_CompanyId_RoleId",
                table: "CompanyRoles",
                columns: new[] { "CompanyId", "RoleId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_CompanyRoles_CompanyId_RoleId",
                table: "CompanyRoles");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_CompanyRoles_CompanyId_RoleId_RoleVersion",
                table: "CompanyRoles",
                columns: new[] { "CompanyId", "RoleId", "RoleVersion" });
        }
    }
}
