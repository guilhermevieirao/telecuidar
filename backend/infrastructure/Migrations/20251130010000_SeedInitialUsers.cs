using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Hash para senha "zxcasd" usando BCrypt
            var medPasswordHash = "$2a$12$6HEKfLVQlqPk.TqY8xJM3uYGvqN7pB0Zf0kZH7Rq4nG9x3yJM5yKG";
            // Hash para senha "zxcasd" usando BCrypt
            var guiPasswordHash = "$2a$12$6HEKfLVQlqPk.TqY8xJM3uYGvqN7pB0Zf0kZH7Rq4nG9x3yJM5yKG";

            // Inserir usuário profissional (med@med.com)
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "FirstName", "LastName", "Email", "PasswordHash", "Role", "EmailConfirmed", "IsActive", "CreatedAt" },
                values: new object[] { 
                    "med", 
                    "med", 
                    "med@med.com", 
                    medPasswordHash,
                    2, // Profissional
                    true, 
                    true, 
                    DateTime.UtcNow 
                });

            // Inserir usuário paciente (gui@gui.com)
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "FirstName", "LastName", "Email", "PasswordHash", "Role", "EmailConfirmed", "IsActive", "CreatedAt" },
                values: new object[] { 
                    "gui", 
                    "gui", 
                    "gui@gui.com", 
                    guiPasswordHash,
                    1, // Paciente
                    true, 
                    true, 
                    DateTime.UtcNow 
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remover os usuários na ordem inversa
            migrationBuilder.Sql("DELETE FROM Users WHERE Email = 'gui@gui.com'");
            migrationBuilder.Sql("DELETE FROM Users WHERE Email = 'med@med.com'");
        }
    }
}
