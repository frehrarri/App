using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voyage.Migrations
{
    /// <inheritdoc />
    public partial class update_TeamUserRoles_Team_CompanyRoles_AK_FK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamUserRoles_Team_CompanyId_TeamId",
                table: "TeamUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_TeamUserRoles_CompanyId_TeamId",
                table: "TeamUserRoles");

            migrationBuilder.CreateIndex(
                name: "IX_TeamUserRoles_TeamKey",
                table: "TeamUserRoles",
                column: "TeamKey");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamUserRoles_Team_TeamKey",
                table: "TeamUserRoles",
                column: "TeamKey",
                principalTable: "Team",
                principalColumn: "TeamKey",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamUserRoles_Team_TeamKey",
                table: "TeamUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_TeamUserRoles_TeamKey",
                table: "TeamUserRoles");

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
    }
}
