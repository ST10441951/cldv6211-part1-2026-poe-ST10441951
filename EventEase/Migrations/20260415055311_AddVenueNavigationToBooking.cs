using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventEase.Migrations
{
    /// <inheritdoc />
    public partial class AddVenueNavigationToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Booking_VenueID",
                table: "Booking",
                column: "VenueID");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Venue_VenueID",
                table: "Booking",
                column: "VenueID",
                principalTable: "Venue",
                principalColumn: "VenueID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Venue_VenueID",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_VenueID",
                table: "Booking");
        }
    }
}
