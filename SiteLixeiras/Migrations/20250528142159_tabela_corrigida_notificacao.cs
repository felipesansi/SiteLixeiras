using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteLixeiras.Migrations
{
    /// <inheritdoc />
    public partial class tabela_corrigida_notificacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataResposta",
                table: "Notificacoes");

            migrationBuilder.DropColumn(
                name: "Resposta",
                table: "Notificacoes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataResposta",
                table: "Notificacoes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Resposta",
                table: "Notificacoes",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
