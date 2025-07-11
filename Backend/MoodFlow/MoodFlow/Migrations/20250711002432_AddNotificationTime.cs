using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoodFlow.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "NotificationTime",
                table: "Users",
                type: "interval",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotificationTime",
                table: "Users");
        }
    }
}
