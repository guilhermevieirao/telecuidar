using Microsoft.EntityFrameworkCore.Migrations;

namespace app.Infrastructure.Migrations
{
    public partial class UpdateModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Adicione aqui comandos de alteração de schema, se necessário
            // Exemplo:
            // migrationBuilder.AddColumn<string>(
            //     name: "NovaColuna",
            //     table: "ScheduleBlocks",
            //     nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reversão das alterações
            // migrationBuilder.DropColumn(
            //     name: "NovaColuna",
            //     table: "ScheduleBlocks");
        }
    }
}
