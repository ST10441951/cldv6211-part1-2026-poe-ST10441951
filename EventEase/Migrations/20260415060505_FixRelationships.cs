using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventEase.Migrations
{
    /// <inheritdoc />
    public partial class FixRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Venue_VenueID",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_Booking_BookingID",
                table: "Event");

            migrationBuilder.AddColumn<int>(
                name: "BookingID1",
                table: "Event",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Event_BookingID1",
                table: "Event",
                column: "BookingID1");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_EventID",
                table: "Booking",
                column: "EventID");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Event_EventID",
                table: "Booking",
                column: "EventID",
                principalTable: "Event",
                principalColumn: "EventID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Venue_VenueID",
                table: "Booking",
                column: "VenueID",
                principalTable: "Venue",
                principalColumn: "VenueID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Booking_BookingID",
                table: "Event",
                column: "BookingID",
                principalTable: "Booking",
                principalColumn: "BookingID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Booking_BookingID1",
                table: "Event",
                column: "BookingID1",
                principalTable: "Booking",
                principalColumn: "BookingID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Event_EventID",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Venue_VenueID",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_Booking_BookingID",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_Booking_BookingID1",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_BookingID1",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Booking_EventID",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "BookingID1",
                table: "Event");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Venue_VenueID",
                table: "Booking",
                column: "VenueID",
                principalTable: "Venue",
                principalColumn: "VenueID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Booking_BookingID",
                table: "Event",
                column: "BookingID",
                principalTable: "Booking",
                principalColumn: "BookingID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
