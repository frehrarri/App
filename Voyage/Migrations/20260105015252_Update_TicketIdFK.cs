using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voyage.Migrations
{
    /// <inheritdoc />
    public partial class Update_TicketIdFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketId_IsLatest_IsActive",
                table: "Tickets",
                columns: new[] { "TicketId", "IsLatest", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketId_TicketVersion",
                table: "Tickets",
                columns: new[] { "TicketId", "TicketVersion" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tickets_TicketId_IsLatest_IsActive",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_TicketId_TicketVersion",
                table: "Tickets");
        }
    }
}
