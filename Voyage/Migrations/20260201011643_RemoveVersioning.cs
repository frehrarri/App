using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

/*
 Removed versions properties from multiple tables and entities. Will work on versioning for each thing separately when base functionality
is completed. Also will handle this differently. When the time comes, create separate tables for the specific table's version history.
 */

#nullable disable

namespace Voyage.Migrations
{
    /// <inheritdoc />
    public partial class RemoveVersioning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentUserRoles_CompanyRoles_RoleId_DeptUserRoleVersion",
                table: "DepartmentUserRoles");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Team_TeamId_TeamVersion",
                table: "Team");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_IndividualUserRoles_CompanyId_EmployeeId_RoleId_IndivUserRo~",
                table: "IndividualUserRoles");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_DepartmentUserRoles_DepartmentKey_CompanyId_EmployeeId_Role~",
                table: "DepartmentUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_DepartmentUserRoles_RoleId_DeptUserRoleVersion",
                table: "DepartmentUserRoles");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Department_DepartmentId_DepartmentVersion",
                table: "Department");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_CompanyRoles_RoleId_RoleVersion",
                table: "CompanyRoles");

            migrationBuilder.DropColumn(
                name: "TeamVersion",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "IndivUserRoleVersion",
                table: "IndividualUserRoles");

            migrationBuilder.DropColumn(
                name: "DeptUserRoleVersion",
                table: "DepartmentUserRoles");

            migrationBuilder.DropColumn(
                name: "DepartmentVersion",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "RoleVersion",
                table: "CompanyRoles");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "Company",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Team_TeamId",
                table: "Team",
                column: "TeamId");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_IndividualUserRoles_CompanyId_EmployeeId_RoleId",
                table: "IndividualUserRoles",
                columns: new[] { "CompanyId", "EmployeeId", "RoleId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_DepartmentUserRoles_DepartmentKey_CompanyId_EmployeeId_Role~",
                table: "DepartmentUserRoles",
                columns: new[] { "DepartmentKey", "CompanyId", "EmployeeId", "RoleId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_CompanyRoles_RoleId",
                table: "CompanyRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentUserRoles_RoleId",
                table: "DepartmentUserRoles",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentUserRoles_CompanyRoles_RoleId",
                table: "DepartmentUserRoles",
                column: "RoleId",
                principalTable: "CompanyRoles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentUserRoles_CompanyRoles_RoleId",
                table: "DepartmentUserRoles");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Team_TeamId",
                table: "Team");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_IndividualUserRoles_CompanyId_EmployeeId_RoleId",
                table: "IndividualUserRoles");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_DepartmentUserRoles_DepartmentKey_CompanyId_EmployeeId_Role~",
                table: "DepartmentUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_DepartmentUserRoles_RoleId",
                table: "DepartmentUserRoles");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_CompanyRoles_RoleId",
                table: "CompanyRoles");

            migrationBuilder.AddColumn<decimal>(
                name: "TeamVersion",
                table: "Team",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "IndivUserRoleVersion",
                table: "IndividualUserRoles",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DeptUserRoleVersion",
                table: "DepartmentUserRoles",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DepartmentVersion",
                table: "Department",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RoleVersion",
                table: "CompanyRoles",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "Company",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Team_TeamId_TeamVersion",
                table: "Team",
                columns: new[] { "TeamId", "TeamVersion" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_IndividualUserRoles_CompanyId_EmployeeId_RoleId_IndivUserRo~",
                table: "IndividualUserRoles",
                columns: new[] { "CompanyId", "EmployeeId", "RoleId", "IndivUserRoleVersion" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_DepartmentUserRoles_DepartmentKey_CompanyId_EmployeeId_Role~",
                table: "DepartmentUserRoles",
                columns: new[] { "DepartmentKey", "CompanyId", "EmployeeId", "RoleId", "DeptUserRoleVersion" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Department_DepartmentId_DepartmentVersion",
                table: "Department",
                columns: new[] { "DepartmentId", "DepartmentVersion" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_CompanyRoles_RoleId_RoleVersion",
                table: "CompanyRoles",
                columns: new[] { "RoleId", "RoleVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentUserRoles_RoleId_DeptUserRoleVersion",
                table: "DepartmentUserRoles",
                columns: new[] { "RoleId", "DeptUserRoleVersion" });

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentUserRoles_CompanyRoles_RoleId_DeptUserRoleVersion",
                table: "DepartmentUserRoles",
                columns: new[] { "RoleId", "DeptUserRoleVersion" },
                principalTable: "CompanyRoles",
                principalColumns: new[] { "RoleId", "RoleVersion" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
