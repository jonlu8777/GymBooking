using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymBooking.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class datePassed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isDatePassed",
                table: "GymClass",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isDatePassed",
                table: "GymClass");
        }
    }
}
