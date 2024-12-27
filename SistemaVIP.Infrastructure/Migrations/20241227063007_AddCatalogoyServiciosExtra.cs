using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaVIP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCatalogoyServiciosExtra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DuracionHoras",
                schema: "SistemaVIP",
                table: "Servicios",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DuracionHoras",
                schema: "SistemaVIP",
                table: "Servicios");
        }
    }
}
