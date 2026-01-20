using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voyage.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFK_Department_Teams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Team_Department_DepartmentId",
                table: "Team");

            migrationBuilder.DropIndex(
                name: "IX_Team_DepartmentId",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Team");

            migrationBuilder.AlterColumn<decimal>(
                name: "TeamVersion",
                table: "Team",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldDefaultValue: 1m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TeamVersion",
                table: "Team",
                type: "numeric",
                nullable: false,
                defaultValue: 1m,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Team",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Team_DepartmentId",
                table: "Team",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Team_Department_DepartmentId",
                table: "Team",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
