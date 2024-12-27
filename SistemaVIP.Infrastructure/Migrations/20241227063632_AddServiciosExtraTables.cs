using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaVIP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddServiciosExtraTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiciosExtraCatalogo",
                schema: "SistemaVIP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiciosExtraCatalogo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiciosExtra",
                schema: "SistemaVIP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServicioTerapeutaId = table.Column<int>(type: "int", nullable: false),
                    ServicioExtraCatalogoId = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notas = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiciosExtra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiciosExtra_ServiciosExtraCatalogo_ServicioExtraCatalogoId",
                        column: x => x.ServicioExtraCatalogoId,
                        principalSchema: "SistemaVIP",
                        principalTable: "ServiciosExtraCatalogo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiciosExtra_ServiciosTerapeutas_ServicioTerapeutaId",
                        column: x => x.ServicioTerapeutaId,
                        principalSchema: "SistemaVIP",
                        principalTable: "ServiciosTerapeutas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosExtra_ServicioExtraCatalogoId",
                schema: "SistemaVIP",
                table: "ServiciosExtra",
                column: "ServicioExtraCatalogoId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosExtra_ServicioTerapeutaId",
                schema: "SistemaVIP",
                table: "ServiciosExtra",
                column: "ServicioTerapeutaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiciosExtra",
                schema: "SistemaVIP");

            migrationBuilder.DropTable(
                name: "ServiciosExtraCatalogo",
                schema: "SistemaVIP");
        }
    }
}
