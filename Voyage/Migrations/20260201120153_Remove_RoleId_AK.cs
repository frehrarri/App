using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voyage.Migrations
{
    /// <inheritdoc />
    public partial class Remove_RoleId_AK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_IndividualUserRoles_CompanyId_EmployeeId_RoleId",
                table: "IndividualUserRoles");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_IndividualUserRoles_CompanyId_EmployeeId",
                table: "IndividualUserRoles",
                columns: new[] { "CompanyId", "EmployeeId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_IndividualUserRoles_CompanyId_EmployeeId",
                table: "IndividualUserRoles");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_IndividualUserRoles_CompanyId_EmployeeId_RoleId",
                table: "IndividualUserRoles",
                columns: new[] { "CompanyId", "EmployeeId", "RoleId" });
        }
    }
}
