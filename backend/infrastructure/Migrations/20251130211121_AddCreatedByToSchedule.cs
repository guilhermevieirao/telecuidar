using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByToSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Schedules",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_CreatedByUserId",
                table: "Schedules",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Users_CreatedByUserId",
                table: "Schedules",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Users_CreatedByUserId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_CreatedByUserId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Schedules");
        }
    }
}
