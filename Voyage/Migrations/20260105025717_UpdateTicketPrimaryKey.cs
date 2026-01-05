using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Voyage.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTicketPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketDetails_Tickets_TicketId",
                table: "TicketDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tickets",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_TicketId_TicketVersion",
                table: "Tickets");

            migrationBuilder.AlterColumn<int>(
                name: "TicketId",
                table: "Tickets",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<decimal>(
                name: "TicketVersion",
                table: "TicketDetails",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tickets",
                table: "Tickets",
                columns: new[] { "TicketId", "TicketVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_TicketDetails_TicketId_TicketVersion",
                table: "TicketDetails",
                columns: new[] { "TicketId", "TicketVersion" });

            migrationBuilder.AddForeignKey(
                name: "FK_TicketDetails_Tickets_TicketId_TicketVersion",
                table: "TicketDetails",
                columns: new[] { "TicketId", "TicketVersion" },
                principalTable: "Tickets",
                principalColumns: new[] { "TicketId", "TicketVersion" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketDetails_Tickets_TicketId_TicketVersion",
                table: "TicketDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tickets",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_TicketDetails_TicketId_TicketVersion",
                table: "TicketDetails");

            migrationBuilder.DropColumn(
                name: "TicketVersion",
                table: "TicketDetails");

            migrationBuilder.AlterColumn<int>(
                name: "TicketId",
                table: "Tickets",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tickets",
                table: "Tickets",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketId_TicketVersion",
                table: "Tickets",
                columns: new[] { "TicketId", "TicketVersion" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketDetails_Tickets_TicketId",
                table: "TicketDetails",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "TicketId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
