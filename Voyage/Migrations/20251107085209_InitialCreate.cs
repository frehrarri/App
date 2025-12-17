using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Voyage.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "_Log",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LogType = table.Column<decimal>(type: "numeric(1)", precision: 1, nullable: false),
                    LogSeverity = table.Column<decimal>(type: "numeric(1)", precision: 1, nullable: false),
                    Message = table.Column<string>(type: "varchar", nullable: false),
                    StackTrace = table.Column<string>(type: "varchar", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Log", x => x.LogId);
                });

            migrationBuilder.CreateTable(
                name: "_Master",
                columns: table => new
                {
                    MasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Master", x => x.MasterId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "varchar(100)", nullable: false),
                    LastName = table.Column<string>(type: "varchar(100)", nullable: false),
                    MiddleName = table.Column<string>(type: "varchar(100)", nullable: false),
                    UserName = table.Column<string>(type: "varchar(20)", nullable: false),
                    Email = table.Column<string>(type: "varchar(254)", nullable: false),
                    Password = table.Column<string>(type: "varchar(20)", nullable: false),
                    PhoneCountryCode = table.Column<decimal>(type: "numeric(3)", precision: 3, nullable: false),
                    PhoneAreaCode = table.Column<decimal>(type: "numeric(5)", precision: 5, nullable: false),
                    Phone = table.Column<decimal>(type: "numeric(10,0)", precision: 10, nullable: false),
                    StreetAddress = table.Column<string>(type: "varchar(75)", nullable: false),
                    UnitNumber = table.Column<decimal>(type: "numeric(5)", precision: 5, nullable: false),
                    Country = table.Column<string>(type: "varchar(50)", nullable: false),
                    State = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "varchar(50)", nullable: false),
                    ZipCode = table.Column<decimal>(type: "numeric(16)", precision: 16, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "_Log");

            migrationBuilder.DropTable(
                name: "_Master");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
