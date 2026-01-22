using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voyage.Migrations
{
    /// <inheritdoc />
    public partial class Remove_CompositeKey_Departments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Department_DepartmentId_DepartmentVersion",
                table: "Department");

            migrationBuilder.CreateIndex(
                name: "IX_Department_DepartmentId",
                table: "Department",
                column: "DepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Department_DepartmentId",
                table: "Department");

            migrationBuilder.CreateIndex(
                name: "IX_Department_DepartmentId_DepartmentVersion",
                table: "Department",
                columns: new[] { "DepartmentId", "DepartmentVersion" },
                unique: true);
        }
    }
}
