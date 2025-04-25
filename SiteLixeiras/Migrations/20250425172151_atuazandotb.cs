using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteLixeiras.Migrations
{
    /// <inheritdoc />
    public partial class atuazandotb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Complemento",
                table: "EnderecoEntrega",
                type: "nvarchar(999)",
                maxLength: 999,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Numero",
                table: "EnderecoEntrega",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Complemento",
                table: "EnderecoEntrega");

            migrationBuilder.DropColumn(
                name: "Numero",
                table: "EnderecoEntrega");
        }
    }
}
