using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpecFlow.PoC.Migrations
{
    /// <inheritdoc />
    public partial class Addonepropertymigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasBeenBroadcast",
                table: "WeatherForecasts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasBeenBroadcast",
                table: "WeatherForecasts");
        }
    }
}
