using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAIFieldsToAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AISummary",
                table: "Appointments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AISummaryGeneratedAt",
                table: "Appointments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AIDiagnosticHypothesis",
                table: "Appointments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AIDiagnosisGeneratedAt",
                table: "Appointments",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AISummary",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "AISummaryGeneratedAt",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "AIDiagnosticHypothesis",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "AIDiagnosisGeneratedAt",
                table: "Appointments");
        }
    }
}
