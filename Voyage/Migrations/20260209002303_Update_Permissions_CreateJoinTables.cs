using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voyage.Migrations
{
    /// <inheritdoc />
    public partial class Update_Permissions_CreateJoinTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPermissions",
                table: "UserPermissions");

            migrationBuilder.DropIndex(
                name: "IX_UserPermissions_Id",
                table: "UserPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamPermissions",
                table: "TeamPermissions");

            migrationBuilder.DropIndex(
                name: "IX_TeamPermissions_TeamKey",
                table: "TeamPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_RoleKey",
                table: "RolePermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DepartmentPermissions",
                table: "DepartmentPermissions");

            migrationBuilder.DropIndex(
                name: "IX_DepartmentPermissions_DepartmentKey",
                table: "DepartmentPermissions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "IsLatest",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "UserPermissionName",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TeamPermissions");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "TeamPermissions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TeamPermissions");

            migrationBuilder.DropColumn(
                name: "IsLatest",
                table: "TeamPermissions");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "TeamPermissions");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "TeamPermissions");

            migrationBuilder.DropColumn(
                name: "TeamPermissionName",
                table: "TeamPermissions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "IsLatest",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "RolePermissionName",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "DepartmentPermissions");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "DepartmentPermissions");

            migrationBuilder.DropColumn(
                name: "DepartmentPermissionName",
                table: "DepartmentPermissions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "DepartmentPermissions");

            migrationBuilder.DropColumn(
                name: "IsLatest",
                table: "DepartmentPermissions");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "DepartmentPermissions");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "DepartmentPermissions");

            migrationBuilder.RenameColumn(
                name: "UserPermissionKey",
                table: "UserPermissions",
                newName: "PermissionKey");

            migrationBuilder.RenameColumn(
                name: "TeamPermissionKey",
                table: "TeamPermissions",
                newName: "PermissionKey");

            migrationBuilder.RenameColumn(
                name: "RolePermissionKey",
                table: "RolePermissions",
                newName: "PermissionKey");

            migrationBuilder.RenameColumn(
                name: "DepartmentPermissionKey",
                table: "DepartmentPermissions",
                newName: "PermissionKey");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "UserPermissions",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPermissions",
                table: "UserPermissions",
                columns: new[] { "Id", "PermissionKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamPermissions",
                table: "TeamPermissions",
                columns: new[] { "TeamKey", "PermissionKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions",
                columns: new[] { "RoleKey", "PermissionKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DepartmentPermissions",
                table: "DepartmentPermissions",
                columns: new[] { "DepartmentKey", "PermissionKey" });

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_PermissionKey",
                table: "UserPermissions",
                column: "PermissionKey");

            migrationBuilder.CreateIndex(
                name: "IX_TeamPermissions_PermissionKey",
                table: "TeamPermissions",
                column: "PermissionKey");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionKey",
                table: "RolePermissions",
                column: "PermissionKey");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentPermissions_PermissionKey",
                table: "DepartmentPermissions",
                column: "PermissionKey");

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentPermissions_Permission_PermissionKey",
                table: "DepartmentPermissions",
                column: "PermissionKey",
                principalTable: "Permission",
                principalColumn: "PermissionKey",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_Permission_PermissionKey",
                table: "RolePermissions",
                column: "PermissionKey",
                principalTable: "Permission",
                principalColumn: "PermissionKey",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamPermissions_Permission_PermissionKey",
                table: "TeamPermissions",
                column: "PermissionKey",
                principalTable: "Permission",
                principalColumn: "PermissionKey",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissions_Permission_PermissionKey",
                table: "UserPermissions",
                column: "PermissionKey",
                principalTable: "Permission",
                principalColumn: "PermissionKey",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentPermissions_Permission_PermissionKey",
                table: "DepartmentPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_Permission_PermissionKey",
                table: "RolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamPermissions_Permission_PermissionKey",
                table: "TeamPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissions_Permission_PermissionKey",
                table: "UserPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPermissions",
                table: "UserPermissions");

            migrationBuilder.DropIndex(
                name: "IX_UserPermissions_PermissionKey",
                table: "UserPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamPermissions",
                table: "TeamPermissions");

            migrationBuilder.DropIndex(
                name: "IX_TeamPermissions_PermissionKey",
                table: "TeamPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_PermissionKey",
                table: "RolePermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DepartmentPermissions",
                table: "DepartmentPermissions");

            migrationBuilder.DropIndex(
                name: "IX_DepartmentPermissions_PermissionKey",
                table: "DepartmentPermissions");

            migrationBuilder.RenameColumn(
                name: "PermissionKey",
                table: "UserPermissions",
                newName: "UserPermissionKey");

            migrationBuilder.RenameColumn(
                name: "PermissionKey",
                table: "TeamPermissions",
                newName: "TeamPermissionKey");

            migrationBuilder.RenameColumn(
                name: "PermissionKey",
                table: "RolePermissions",
                newName: "RolePermissionKey");

            migrationBuilder.RenameColumn(
                name: "PermissionKey",
                table: "DepartmentPermissions",
                newName: "DepartmentPermissionKey");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "UserPermissions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "UserPermissions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "UserPermissions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserPermissions",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLatest",
                table: "UserPermissions",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "UserPermissions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "UserPermissions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserPermissionName",
                table: "UserPermissions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "TeamPermissions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "TeamPermissions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TeamPermissions",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLatest",
                table: "TeamPermissions",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "TeamPermissions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "TeamPermissions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeamPermissionName",
                table: "TeamPermissions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "RolePermissions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "RolePermissions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "RolePermissions",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLatest",
                table: "RolePermissions",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "RolePermissions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "RolePermissions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RolePermissionName",
                table: "RolePermissions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "DepartmentPermissions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "DepartmentPermissions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentPermissionName",
                table: "DepartmentPermissions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "DepartmentPermissions",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLatest",
                table: "DepartmentPermissions",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "DepartmentPermissions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "DepartmentPermissions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPermissions",
                table: "UserPermissions",
                column: "UserPermissionKey");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamPermissions",
                table: "TeamPermissions",
                column: "TeamPermissionKey");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions",
                column: "RolePermissionKey");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DepartmentPermissions",
                table: "DepartmentPermissions",
                column: "DepartmentPermissionKey");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_Id",
                table: "UserPermissions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TeamPermissions_TeamKey",
                table: "TeamPermissions",
                column: "TeamKey");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleKey",
                table: "RolePermissions",
                column: "RoleKey");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentPermissions_DepartmentKey",
                table: "DepartmentPermissions",
                column: "DepartmentKey");
        }
    }
}
