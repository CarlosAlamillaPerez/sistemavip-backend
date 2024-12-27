using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaVIP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCancelacionesPresentador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CancelacionesPresentador",
                schema: "SistemaVIP",
                columns: table => new
                {
                    PresentadorId = table.Column<int>(type: "int", nullable: false),
                    NombrePresentador = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CantidadCancelaciones = table.Column<int>(type: "int", nullable: false),
                    SemanaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SemanaFin = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CancelacionesPresentador", x => x.PresentadorId);
                    table.ForeignKey(
                        name: "FK_CancelacionesPresentador_Presentadores_PresentadorId",
                        column: x => x.PresentadorId,
                        principalSchema: "SistemaVIP",
                        principalTable: "Presentadores",
                        principalColumn: "Id");
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CancelacionesPresentador",
                schema: "SistemaVIP");
        }
    }
}
