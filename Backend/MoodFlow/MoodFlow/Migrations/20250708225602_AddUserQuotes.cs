using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoodFlow.Migrations
{
    /// <inheritdoc />
    public partial class AddUserQuotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserQuotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    QuoteId = table.Column<int>(type: "integer", nullable: false)
                    
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserQuotes", x => new { x.UserId, x.QuoteId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserQuotes");
        }
    }
}
