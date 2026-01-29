using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voyage.Migrations
{
    /// <inheritdoc />
    public partial class Remove_AK_CompanyRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_CompanyRoles_CompanyId_RoleId",
                table: "CompanyRoles");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyRoles_CompanyId",
                table: "CompanyRoles",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CompanyRoles_CompanyId",
                table: "CompanyRoles");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_CompanyRoles_CompanyId_RoleId",
                table: "CompanyRoles",
                columns: new[] { "CompanyId", "RoleId" });
        }
    }
}
