using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaVIP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EliminandoCamposRedundantes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiciosTerapeutas_AspNetUsers_IdPresentadorConfirmaPago",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas");

            migrationBuilder.DropTable(
                name: "CambioEstadoServicioModel",
                schema: "SistemaVIP");

            migrationBuilder.DropIndex(
                name: "IX_ServiciosTerapeutas_IdPresentadorConfirmaPago",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas");

            migrationBuilder.DropColumn(
                name: "EvidenciaTransporte",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas");

            migrationBuilder.DropColumn(
                name: "FechaComprobantePago",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas");

            migrationBuilder.DropColumn(
                name: "FechaRegistroGastosTransporte",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas");

            migrationBuilder.DropColumn(
                name: "GastosTransporte",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas");

            migrationBuilder.DropColumn(
                name: "IdPresentadorConfirmaPago",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas");

            migrationBuilder.DropColumn(
                name: "MontoEfectivo",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas");

            migrationBuilder.DropColumn(
                name: "MontoTransferencia",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas");

            migrationBuilder.DropColumn(
                name: "NotasPago",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas");

            migrationBuilder.DropColumn(
                name: "NotasTransporte",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas");

            migrationBuilder.DropColumn(
                name: "UrlComprobantePago",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EvidenciaTransporte",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaComprobantePago",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistroGastosTransporte",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GastosTransporte",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdPresentadorConfirmaPago",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoEfectivo",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoTransferencia",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NotasPago",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NotasTransporte",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UrlComprobantePago",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CambioEstadoServicioModel",
                schema: "SistemaVIP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServicioTerapeutaId = table.Column<int>(type: "int", nullable: false),
                    UsuarioCambioId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EstadoAnterior = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstadoNuevo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCambio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdUsuarioCambio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MotivosCambio = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CambioEstadoServicioModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CambioEstadoServicioModel_AspNetUsers_UsuarioCambioId",
                        column: x => x.UsuarioCambioId,
                        principalSchema: "SistemaVIP",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CambioEstadoServicioModel_ServiciosTerapeutas_ServicioTerapeutaId",
                        column: x => x.ServicioTerapeutaId,
                        principalSchema: "SistemaVIP",
                        principalTable: "ServiciosTerapeutas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosTerapeutas_IdPresentadorConfirmaPago",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                column: "IdPresentadorConfirmaPago");

            migrationBuilder.CreateIndex(
                name: "IX_CambioEstadoServicioModel_ServicioTerapeutaId",
                schema: "SistemaVIP",
                table: "CambioEstadoServicioModel",
                column: "ServicioTerapeutaId");

            migrationBuilder.CreateIndex(
                name: "IX_CambioEstadoServicioModel_UsuarioCambioId",
                schema: "SistemaVIP",
                table: "CambioEstadoServicioModel",
                column: "UsuarioCambioId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiciosTerapeutas_AspNetUsers_IdPresentadorConfirmaPago",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                column: "IdPresentadorConfirmaPago",
                principalSchema: "SistemaVIP",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
