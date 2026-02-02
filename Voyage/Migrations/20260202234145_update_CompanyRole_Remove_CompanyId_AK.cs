using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voyage.Migrations
{
    /// <inheritdoc />
    public partial class update_CompanyRole_Remove_CompanyId_AK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamUserRoles_CompanyRoles_CompanyId_RoleId",
                table: "TeamUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_TeamUserRoles_CompanyId_RoleId",
                table: "TeamUserRoles");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_CompanyRoles_CompanyId_RoleId",
                table: "CompanyRoles");

            migrationBuilder.CreateIndex(
                name: "IX_TeamUserRoles_RoleId",
                table: "TeamUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyRoles_CompanyId",
                table: "CompanyRoles",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamUserRoles_CompanyRoles_RoleId",
                table: "TeamUserRoles",
                column: "RoleId",
                principalTable: "CompanyRoles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamUserRoles_CompanyRoles_RoleId",
                table: "TeamUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_TeamUserRoles_RoleId",
                table: "TeamUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_CompanyRoles_CompanyId",
                table: "CompanyRoles");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_CompanyRoles_CompanyId_RoleId",
                table: "CompanyRoles",
                columns: new[] { "CompanyId", "RoleId" });

            migrationBuilder.CreateIndex(
                name: "IX_TeamUserRoles_CompanyId_RoleId",
                table: "TeamUserRoles",
                columns: new[] { "CompanyId", "RoleId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TeamUserRoles_CompanyRoles_CompanyId_RoleId",
                table: "TeamUserRoles",
                columns: new[] { "CompanyId", "RoleId" },
                principalTable: "CompanyRoles",
                principalColumns: new[] { "CompanyId", "RoleId" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
