using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hotelbooking.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerSnapshotToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerEmailSnapshot",
                table: "Bookings",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerFirstNameSnapshot",
                table: "Bookings",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerLastNameSnapshot",
                table: "Bookings",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerEmailSnapshot",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerFirstNameSnapshot",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerLastNameSnapshot",
                table: "Bookings");
        }
    }
}
