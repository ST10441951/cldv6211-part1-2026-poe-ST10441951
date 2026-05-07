using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventEase.Migrations
{
    /// <inheritdoc />
    public partial class RemovedBookingIdFromEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Booking_BookingID",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_BookingID",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "BookingID",
                table: "Event");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookingID",
                table: "Event",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Event_BookingID",
                table: "Event",
                column: "BookingID");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Booking_BookingID",
                table: "Event",
                column: "BookingID",
                principalTable: "Booking",
                principalColumn: "BookingID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
