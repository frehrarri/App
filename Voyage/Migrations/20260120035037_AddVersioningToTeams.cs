using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voyage.Migrations
{
    /// <inheritdoc />
    public partial class AddVersioningToTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_Team_TeamId",
                table: "TeamMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamMembers",
                table: "TeamMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Team",
                table: "Team");

            migrationBuilder.AddColumn<decimal>(
                name: "TeamVersion",
                table: "TeamMembers",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TeamVersion",
                table: "Team",
                type: "numeric",
                nullable: false,
                defaultValue: 1m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamMembers",
                table: "TeamMembers",
                columns: new[] { "TeamId", "TeamVersion", "CompanyId", "EmployeeId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Team",
                table: "Team",
                columns: new[] { "TeamId", "TeamVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_TeamVersion",
                table: "TeamMembers",
                columns: new[] { "TeamId", "TeamVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_Team_Latest",
                table: "Team",
                columns: new[] { "TeamId", "IsLatest" });

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_Team_TeamId_TeamVersion",
                table: "TeamMembers",
                columns: new[] { "TeamId", "TeamVersion" },
                principalTable: "Team",
                principalColumns: new[] { "TeamId", "TeamVersion" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_Team_TeamId_TeamVersion",
                table: "TeamMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamMembers",
                table: "TeamMembers");

            migrationBuilder.DropIndex(
                name: "IX_TeamMember_TeamVersion",
                table: "TeamMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Team",
                table: "Team");

            migrationBuilder.DropIndex(
                name: "IX_Team_Latest",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "TeamVersion",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "TeamVersion",
                table: "Team");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamMembers",
                table: "TeamMembers",
                columns: new[] { "TeamId", "CompanyId", "EmployeeId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Team",
                table: "Team",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_Team_TeamId",
                table: "TeamMembers",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "TeamId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
