using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpecFlow.PoC.Migrations
{
    /// <inheritdoc />
    public partial class Addonepropertymigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "WeatherForecasts",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "WeatherForecasts");
        }
    }
}
