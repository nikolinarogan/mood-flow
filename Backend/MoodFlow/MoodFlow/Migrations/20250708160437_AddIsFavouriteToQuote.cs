using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoodFlow.Migrations
{
    /// <inheritdoc />
    public partial class AddIsFavouriteToQuote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFavourite",
                table: "Quotes",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFavourite",
                table: "Quotes");
        }
    }
}
