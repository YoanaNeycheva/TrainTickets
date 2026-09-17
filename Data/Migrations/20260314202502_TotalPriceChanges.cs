using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainTickets.Data.Migrations
{
    /// <inheritdoc />
    public partial class TotalPriceChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TotalPrice",
                table: "Tickets",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Tickets");
        }
    }
}
