using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voyage.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyId_RolePErmissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions");

            migrationBuilder.AddColumn<bool>(
                name: "InheritIsDenied",
                table: "UserPermissions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "RolePermissions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions",
                columns: new[] { "CompanyId", "RoleKey", "PermissionKey" });

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_Id_PermissionKey",
                table: "UserPermissions",
                columns: new[] { "Id", "PermissionKey" });

            migrationBuilder.CreateIndex(
                name: "IX_TeamPermissions_TeamKey_PermissionKey",
                table: "TeamPermissions",
                columns: new[] { "TeamKey", "PermissionKey" });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_CompanyId_RoleKey_PermissionKey",
                table: "RolePermissions",
                columns: new[] { "CompanyId", "RoleKey", "PermissionKey" });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleKey",
                table: "RolePermissions",
                column: "RoleKey");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentPermissions_DepartmentKey_PermissionKey",
                table: "DepartmentPermissions",
                columns: new[] { "DepartmentKey", "PermissionKey" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserPermissions_Id_PermissionKey",
                table: "UserPermissions");

            migrationBuilder.DropIndex(
                name: "IX_TeamPermissions_TeamKey_PermissionKey",
                table: "TeamPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_CompanyId_RoleKey_PermissionKey",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_RoleKey",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_DepartmentPermissions_DepartmentKey_PermissionKey",
                table: "DepartmentPermissions");

            migrationBuilder.DropColumn(
                name: "InheritIsDenied",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "RolePermissions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions",
                columns: new[] { "RoleKey", "PermissionKey" });
        }
    }
}
