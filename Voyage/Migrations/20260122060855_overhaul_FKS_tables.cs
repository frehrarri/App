using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Voyage.Migrations
{
    /// <inheritdoc />
    public partial class overhaul_FKS_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentRole_Department_DepartmentId",
                table: "DepartmentRole");

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

            migrationBuilder.DropPrimaryKey(
                name: "PK_DepartmentRole",
                table: "DepartmentRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Department",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "TeamVersion",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "DepartmentRole");

            migrationBuilder.AddColumn<Guid>(
                name: "TeamMemberKey",
                table: "TeamMembers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TeamKey",
                table: "TeamMembers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<decimal>(
                name: "TeamVersion",
                table: "Team",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<int>(
                name: "TeamId",
                table: "Team",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<Guid>(
                name: "TeamKey",
                table: "Team",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentRoleKey",
                table: "DepartmentRole",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentKey",
                table: "DepartmentRole",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "Department",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentKey",
                table: "Department",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "DepartmentVersion",
                table: "Department",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamMembers",
                table: "TeamMembers",
                column: "TeamMemberKey");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Team",
                table: "Team",
                column: "TeamKey");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DepartmentRole",
                table: "DepartmentRole",
                column: "DepartmentRoleKey");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Department",
                table: "Department",
                column: "DepartmentKey");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_TeamKey",
                table: "TeamMembers",
                column: "TeamKey");

            migrationBuilder.CreateIndex(
                name: "IX_Team_TeamId_TeamVersion",
                table: "Team",
                columns: new[] { "TeamId", "TeamVersion" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentRole_DepartmentKey",
                table: "DepartmentRole",
                column: "DepartmentKey");

            migrationBuilder.CreateIndex(
                name: "IX_Department_DepartmentId_DepartmentVersion",
                table: "Department",
                columns: new[] { "DepartmentId", "DepartmentVersion" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentRole_Department_DepartmentKey",
                table: "DepartmentRole",
                column: "DepartmentKey",
                principalTable: "Department",
                principalColumn: "DepartmentKey",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_Team_TeamKey",
                table: "TeamMembers",
                column: "TeamKey",
                principalTable: "Team",
                principalColumn: "TeamKey",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentRole_Department_DepartmentKey",
                table: "DepartmentRole");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_Team_TeamKey",
                table: "TeamMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamMembers",
                table: "TeamMembers");

            migrationBuilder.DropIndex(
                name: "IX_TeamMember_TeamKey",
                table: "TeamMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Team",
                table: "Team");

            migrationBuilder.DropIndex(
                name: "IX_Team_TeamId_TeamVersion",
                table: "Team");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DepartmentRole",
                table: "DepartmentRole");

            migrationBuilder.DropIndex(
                name: "IX_DepartmentRole_DepartmentKey",
                table: "DepartmentRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Department",
                table: "Department");

            migrationBuilder.DropIndex(
                name: "IX_Department_DepartmentId_DepartmentVersion",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "TeamMemberKey",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "TeamKey",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "TeamKey",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "DepartmentRoleKey",
                table: "DepartmentRole");

            migrationBuilder.DropColumn(
                name: "DepartmentKey",
                table: "DepartmentRole");

            migrationBuilder.DropColumn(
                name: "DepartmentKey",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "DepartmentVersion",
                table: "Department");

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "TeamMembers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TeamVersion",
                table: "TeamMembers",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "TeamVersion",
                table: "Team",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)",
                oldPrecision: 5,
                oldScale: 2);

            migrationBuilder.AlterColumn<int>(
                name: "TeamId",
                table: "Team",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "DepartmentRole",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "Department",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamMembers",
                table: "TeamMembers",
                columns: new[] { "TeamId", "TeamVersion", "CompanyId", "EmployeeId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Team",
                table: "Team",
                columns: new[] { "TeamId", "TeamVersion" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DepartmentRole",
                table: "DepartmentRole",
                columns: new[] { "DepartmentId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Department",
                table: "Department",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_TeamVersion",
                table: "TeamMembers",
                columns: new[] { "TeamId", "TeamVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_Team_Latest",
                table: "Team",
                columns: new[] { "TeamId", "IsLatest" });

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentRole_Department_DepartmentId",
                table: "DepartmentRole",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_Team_TeamId_TeamVersion",
                table: "TeamMembers",
                columns: new[] { "TeamId", "TeamVersion" },
                principalTable: "Team",
                principalColumns: new[] { "TeamId", "TeamVersion" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
