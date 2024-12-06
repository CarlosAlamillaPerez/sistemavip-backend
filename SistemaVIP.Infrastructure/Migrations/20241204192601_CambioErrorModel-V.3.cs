using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace SistemaVIP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CambioErrorModelV3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bitacora",
                schema: "SistemaVIP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tabla = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValoresAnteriores = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValoresNuevos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bitacora", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bitacora_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalSchema: "SistemaVIP",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Blacklist",
                schema: "SistemaVIP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Motivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdUsuarioRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioRegistroId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blacklist", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Blacklist_AspNetUsers_UsuarioRegistroId",
                        column: x => x.UsuarioRegistroId,
                        principalSchema: "SistemaVIP",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracionComisiones",
                schema: "SistemaVIP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoComision = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PorcentajeEmpresa = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PorcentajePresentador = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionComisiones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiguracionComisiones_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "SistemaVIP",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Presentadores",
                schema: "SistemaVIP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PorcentajeComision = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentoIdentidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notas = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presentadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Presentadores_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "SistemaVIP",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Terapeutas",
                schema: "SistemaVIP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estatura = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentoIdentidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TarifaBase = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TarifaExtra = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terapeutas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Terapeutas_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "SistemaVIP",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Servicios",
                schema: "SistemaVIP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PresentadorId = table.Column<int>(type: "int", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaServicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoServicio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MontoTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCancelacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MotivoCancelacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdUsuarioCancelacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotasCancelacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioCancelacionId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Servicios_AspNetUsers_UsuarioCancelacionId",
                        column: x => x.UsuarioCancelacionId,
                        principalSchema: "SistemaVIP",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Servicios_Presentadores_PresentadorId",
                        column: x => x.PresentadorId,
                        principalSchema: "SistemaVIP",
                        principalTable: "Presentadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TerapeutasPresentadores",
                schema: "SistemaVIP",
                columns: table => new
                {
                    TerapeutaId = table.Column<int>(type: "int", nullable: false),
                    PresentadorId = table.Column<int>(type: "int", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerapeutasPresentadores", x => new { x.TerapeutaId, x.PresentadorId });
                    table.ForeignKey(
                        name: "FK_TerapeutasPresentadores_Presentadores_PresentadorId",
                        column: x => x.PresentadorId,
                        principalSchema: "SistemaVIP",
                        principalTable: "Presentadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TerapeutasPresentadores_Terapeutas_TerapeutaId",
                        column: x => x.TerapeutaId,
                        principalSchema: "SistemaVIP",
                        principalTable: "Terapeutas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comisiones",
                schema: "SistemaVIP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServicioId = table.Column<int>(type: "int", nullable: false),
                    TerapeutaId = table.Column<int>(type: "int", nullable: false),
                    PresentadorId = table.Column<int>(type: "int", nullable: false),
                    MontoTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoTerapeuta = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoComisionTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoComisionEmpresa = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoComisionPresentador = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PorcentajeAplicadoEmpresa = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PorcentajeAplicadoPresentador = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaCalculo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comisiones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comisiones_Presentadores_PresentadorId",
                        column: x => x.PresentadorId,
                        principalSchema: "SistemaVIP",
                        principalTable: "Presentadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comisiones_Servicios_ServicioId",
                        column: x => x.ServicioId,
                        principalSchema: "SistemaVIP",
                        principalTable: "Servicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comisiones_Terapeutas_TerapeutaId",
                        column: x => x.TerapeutaId,
                        principalSchema: "SistemaVIP",
                        principalTable: "Terapeutas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pagos",
                schema: "SistemaVIP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServicioId = table.Column<int>(type: "int", nullable: false),
                    TipoTransferencia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumeroOperacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdUsuarioRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaValidacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdUsuarioValidacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotasValidacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioRegistroId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UsuarioValidacionId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pagos_AspNetUsers_UsuarioRegistroId",
                        column: x => x.UsuarioRegistroId,
                        principalSchema: "SistemaVIP",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Pagos_AspNetUsers_UsuarioValidacionId",
                        column: x => x.UsuarioValidacionId,
                        principalSchema: "SistemaVIP",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Pagos_Servicios_ServicioId",
                        column: x => x.ServicioId,
                        principalSchema: "SistemaVIP",
                        principalTable: "Servicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiciosTerapeutas",
                schema: "SistemaVIP",
                columns: table => new
                {
                    ServicioId = table.Column<int>(type: "int", nullable: false),
                    TerapeutaId = table.Column<int>(type: "int", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UbicacionInicio = table.Column<Point>(type: "geography", nullable: true),
                    UbicacionFin = table.Column<Point>(type: "geography", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MontoTerapeuta = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LinkConfirmacion = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LinkFinalizacion = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComprobantePagoTerapeuta = table.Column<bool>(type: "bit", nullable: false),
                    FechaComprobantePago = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NumeroMovimientoBancario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoMovimiento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UrlComprobanteTransferencia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdPresentadorConfirmaPago = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotasPago = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PresentadorConfirmaPagoId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiciosTerapeutas", x => new { x.ServicioId, x.TerapeutaId });
                    table.ForeignKey(
                        name: "FK_ServiciosTerapeutas_AspNetUsers_PresentadorConfirmaPagoId",
                        column: x => x.PresentadorConfirmaPagoId,
                        principalSchema: "SistemaVIP",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ServiciosTerapeutas_Servicios_ServicioId",
                        column: x => x.ServicioId,
                        principalSchema: "SistemaVIP",
                        principalTable: "Servicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiciosTerapeutas_Terapeutas_TerapeutaId",
                        column: x => x.TerapeutaId,
                        principalSchema: "SistemaVIP",
                        principalTable: "Terapeutas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bitacora_UsuarioId",
                schema: "SistemaVIP",
                table: "Bitacora",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Blacklist_UsuarioRegistroId",
                schema: "SistemaVIP",
                table: "Blacklist",
                column: "UsuarioRegistroId");

            migrationBuilder.CreateIndex(
                name: "IX_Comisiones_PresentadorId",
                schema: "SistemaVIP",
                table: "Comisiones",
                column: "PresentadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comisiones_ServicioId",
                schema: "SistemaVIP",
                table: "Comisiones",
                column: "ServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_Comisiones_TerapeutaId",
                schema: "SistemaVIP",
                table: "Comisiones",
                column: "TerapeutaId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionComisiones_UserId",
                schema: "SistemaVIP",
                table: "ConfiguracionComisiones",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_ServicioId",
                schema: "SistemaVIP",
                table: "Pagos",
                column: "ServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_UsuarioRegistroId",
                schema: "SistemaVIP",
                table: "Pagos",
                column: "UsuarioRegistroId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_UsuarioValidacionId",
                schema: "SistemaVIP",
                table: "Pagos",
                column: "UsuarioValidacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Presentadores_UserId",
                schema: "SistemaVIP",
                table: "Presentadores",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_PresentadorId",
                schema: "SistemaVIP",
                table: "Servicios",
                column: "PresentadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_UsuarioCancelacionId",
                schema: "SistemaVIP",
                table: "Servicios",
                column: "UsuarioCancelacionId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosTerapeutas_PresentadorConfirmaPagoId",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                column: "PresentadorConfirmaPagoId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosTerapeutas_TerapeutaId",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                column: "TerapeutaId");

            migrationBuilder.CreateIndex(
                name: "IX_Terapeutas_UserId",
                schema: "SistemaVIP",
                table: "Terapeutas",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TerapeutasPresentadores_PresentadorId",
                schema: "SistemaVIP",
                table: "TerapeutasPresentadores",
                column: "PresentadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bitacora",
                schema: "SistemaVIP");

            migrationBuilder.DropTable(
                name: "Blacklist",
                schema: "SistemaVIP");

            migrationBuilder.DropTable(
                name: "Comisiones",
                schema: "SistemaVIP");

            migrationBuilder.DropTable(
                name: "ConfiguracionComisiones",
                schema: "SistemaVIP");

            migrationBuilder.DropTable(
                name: "Pagos",
                schema: "SistemaVIP");

            migrationBuilder.DropTable(
                name: "ServiciosTerapeutas",
                schema: "SistemaVIP");

            migrationBuilder.DropTable(
                name: "TerapeutasPresentadores",
                schema: "SistemaVIP");

            migrationBuilder.DropTable(
                name: "Servicios",
                schema: "SistemaVIP");

            migrationBuilder.DropTable(
                name: "Terapeutas",
                schema: "SistemaVIP");

            migrationBuilder.DropTable(
                name: "Presentadores",
                schema: "SistemaVIP");
        }
    }
}
