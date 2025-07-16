using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoodFlow.Migrations
{
    /// <inheritdoc />
    public partial class AddUserMeditationExercises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserMeditationExercises",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    MeditationExerciseId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMeditationExercises", x => new { x.UserId, x.MeditationExerciseId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserMeditationExercises");
        }
    }
}
