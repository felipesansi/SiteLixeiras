using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteLixeiras.Migrations
{
    /// <inheritdoc />
    public partial class add_campo_pedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StatusPagamento",
                table: "Pedidos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusPagamento",
                table: "Pedidos");
        }
    }
}
