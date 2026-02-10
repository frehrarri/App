using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voyage.Migrations
{
    /// <inheritdoc />
    public partial class add_rolepermissions_isenabled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "UserPermissions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "TeamPermissions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "RolePermissions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "DepartmentPermissions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "TeamPermissions");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "DepartmentPermissions");
        }
    }
}
