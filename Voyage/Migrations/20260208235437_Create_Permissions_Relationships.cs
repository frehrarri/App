using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voyage.Migrations
{
    /// <inheritdoc />
    public partial class Create_Permissions_Relationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "UserPermissions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TeamKey",
                table: "TeamPermissions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RoleKey",
                table: "RolePermissions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentKey",
                table: "DepartmentPermissions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentPermissions_Department_DepartmentKey",
                table: "DepartmentPermissions",
                column: "DepartmentKey",
                principalTable: "Department",
                principalColumn: "DepartmentKey",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_CompanyRoles_RoleKey",
                table: "RolePermissions",
                column: "RoleKey",
                principalTable: "CompanyRoles",
                principalColumn: "RoleKey",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamPermissions_Team_TeamKey",
                table: "TeamPermissions",
                column: "TeamKey",
                principalTable: "Team",
                principalColumn: "TeamKey",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissions_AspNetUsers_Id",
                table: "UserPermissions",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentPermissions_Department_DepartmentKey",
                table: "DepartmentPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_CompanyRoles_RoleKey",
                table: "RolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamPermissions_Team_TeamKey",
                table: "TeamPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissions_AspNetUsers_Id",
                table: "UserPermissions");

            migrationBuilder.DropIndex(
                name: "IX_UserPermissions_Id",
                table: "UserPermissions");

            migrationBuilder.DropIndex(
                name: "IX_TeamPermissions_TeamKey",
                table: "TeamPermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_RoleKey",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_DepartmentPermissions_DepartmentKey",
                table: "DepartmentPermissions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "TeamKey",
                table: "TeamPermissions");

            migrationBuilder.DropColumn(
                name: "RoleKey",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "DepartmentKey",
                table: "DepartmentPermissions");
        }
    }
}
