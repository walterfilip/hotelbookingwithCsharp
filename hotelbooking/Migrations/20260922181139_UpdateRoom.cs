using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hotelbooking.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Rooms",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Rooms",
                type: "longtext",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Rooms");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Rooms",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
