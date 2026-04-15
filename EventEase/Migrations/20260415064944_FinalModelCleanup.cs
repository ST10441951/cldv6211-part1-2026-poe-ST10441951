using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventEase.Migrations
{
    /// <inheritdoc />
    public partial class FinalModelCleanup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Booking_BookingID1",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_Venue_VenueID",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_BookingID1",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_VenueID",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "BookingID1",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "VenueID",
                table: "Event");

            migrationBuilder.AddColumn<int>(
                name: "VenueID1",
                table: "Booking",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Booking_VenueID1",
                table: "Booking",
                column: "VenueID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Venue_VenueID1",
                table: "Booking",
                column: "VenueID1",
                principalTable: "Venue",
                principalColumn: "VenueID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Venue_VenueID1",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_VenueID1",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "VenueID1",
                table: "Booking");

            migrationBuilder.AddColumn<int>(
                name: "BookingID1",
                table: "Event",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VenueID",
                table: "Event",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Event_BookingID1",
                table: "Event",
                column: "BookingID1");

            migrationBuilder.CreateIndex(
                name: "IX_Event_VenueID",
                table: "Event",
                column: "VenueID");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Booking_BookingID1",
                table: "Event",
                column: "BookingID1",
                principalTable: "Booking",
                principalColumn: "BookingID");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Venue_VenueID",
                table: "Event",
                column: "VenueID",
                principalTable: "Venue",
                principalColumn: "VenueID");
        }
    }
}
