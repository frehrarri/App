using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Voyage.Migrations
{
    /// <inheritdoc />
    public partial class Remove_TeamsVersion_Constraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Team_TeamId_TeamVersion",
                table: "Team");

            migrationBuilder.AlterColumn<int>(
                name: "TeamId",
                table: "Team",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_Team_TeamId",
                table: "Team",
                column: "TeamId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Team_TeamId",
                table: "Team");

            migrationBuilder.AlterColumn<int>(
                name: "TeamId",
                table: "Team",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_Team_TeamId_TeamVersion",
                table: "Team",
                columns: new[] { "TeamId", "TeamVersion" },
                unique: true);
        }
    }
}
