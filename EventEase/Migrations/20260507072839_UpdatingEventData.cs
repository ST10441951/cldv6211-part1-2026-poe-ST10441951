using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventEase.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingEventData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VenueID",
                table: "Event",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Event_VenueID",
                table: "Event",
                column: "VenueID");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Venue_VenueID",
                table: "Event",
                column: "VenueID",
                principalTable: "Venue",
                principalColumn: "VenueID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Venue_VenueID",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_VenueID",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "VenueID",
                table: "Event");
        }
    }
}
