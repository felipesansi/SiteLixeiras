using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteLixeiras.Migrations
{
    /// <inheritdoc />
    public partial class AdicionandoTb_ProdutoKitItens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EhKit",
                table: "Produtos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "profundidade",
                table: "Produtos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "ProdutoKitItens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProdutoKitId = table.Column<int>(type: "int", nullable: false),
                    ProdutoFilhoId = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutoKitItens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProdutoKitItens_Produtos_ProdutoFilhoId",
                        column: x => x.ProdutoFilhoId,
                        principalTable: "Produtos",
                        principalColumn: "Id_Produto",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProdutoKitItens_Produtos_ProdutoKitId",
                        column: x => x.ProdutoKitId,
                        principalTable: "Produtos",
                        principalColumn: "Id_Produto",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoKitItens_ProdutoFilhoId",
                table: "ProdutoKitItens",
                column: "ProdutoFilhoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoKitItens_ProdutoKitId",
                table: "ProdutoKitItens",
                column: "ProdutoKitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProdutoKitItens");

            migrationBuilder.DropColumn(
                name: "EhKit",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "profundidade",
                table: "Produtos");
        }
    }
}
