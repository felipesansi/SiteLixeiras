using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteLixeiras.Migrations
{
    /// <inheritdoc />
    public partial class corrigindoTabela : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NotificacaoPaiId",
                table: "Notificacoes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notificacoes_NotificacaoPaiId",
                table: "Notificacoes",
                column: "NotificacaoPaiId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notificacoes_Notificacoes_NotificacaoPaiId",
                table: "Notificacoes",
                column: "NotificacaoPaiId",
                principalTable: "Notificacoes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notificacoes_Notificacoes_NotificacaoPaiId",
                table: "Notificacoes");

            migrationBuilder.DropIndex(
                name: "IX_Notificacoes_NotificacaoPaiId",
                table: "Notificacoes");

            migrationBuilder.DropColumn(
                name: "NotificacaoPaiId",
                table: "Notificacoes");
        }
    }
}
