using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteLixeiras.Migrations
{
    /// <inheritdoc />
    public partial class atualizado_carrinho : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "CarrinhoCompraItens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "CarrinhoCompraItens");
        }
    }
}
