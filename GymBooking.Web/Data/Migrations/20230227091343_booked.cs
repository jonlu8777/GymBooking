using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymBooking.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class booked : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "isBookedLabel",
                table: "GymClass",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isBookedLabel",
                table: "GymClass");
        }
    }
}
