using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteLixeiras.Migrations
{
    /// <inheritdoc />
    public partial class AtualizandoTbEnderecos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "EnderecoEntrega");

            migrationBuilder.RenameColumn(
                name: "Endereco",
                table: "EnderecoEntrega",
                newName: "Rua");

            migrationBuilder.AddColumn<string>(
                name: "Bairro",
                table: "EnderecoEntrega",
                type: "nvarchar(999)",
                maxLength: 999,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bairro",
                table: "EnderecoEntrega");

            migrationBuilder.RenameColumn(
                name: "Rua",
                table: "EnderecoEntrega",
                newName: "Endereco");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "EnderecoEntrega",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
