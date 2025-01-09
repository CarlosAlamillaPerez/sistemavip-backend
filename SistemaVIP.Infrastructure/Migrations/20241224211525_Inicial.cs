using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace SistemaVIP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "SistemaVIP");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                schema: "SistemaVIP",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                schema: "SistemaVIP",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                schema: "SistemaVIP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "SistemaVIP",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                schema: "SistemaVIP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "SistemaVIP",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                schema: "SistemaVIP",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "SistemaVIP",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                schema: "SistemaVIP",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "SistemaVIP",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "SistemaVIP",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                schema: "SistemaVIP",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "SistemaVIP",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bitacora",
                schema: "SistemaVIP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tabla = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValoresAnteriores = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValoresNuevos = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bitacora", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bitacora_AspNetUsers_IdUsuario",
                        column: x => x.IdUsuario,
                        principalSchema: "SistemaVIP",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    IdUsuarioRegistro = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notas = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blacklist", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Blacklist_AspNetUsers_IdUsuarioRegistro",
                        column: x => x.IdUsuarioRegistro,
                        principalSchema: "SistemaVIP",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    PorcentajeComision = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCambioEstado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MotivoEstado = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                        onDelete: ReferentialAction.Restrict);
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
                    FechaCambioEstado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MotivoEstado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estatura = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentoIdentidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TarifaBase = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    TarifaExtra = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terapeutas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Terapeutas_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "SistemaVIP",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    TipoUbicacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MontoTotal = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    GastosTransporte = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    NotasTransporte = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCancelacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MotivoCancelacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdUsuarioCancelacion = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NotasCancelacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notas = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Servicios_AspNetUsers_IdUsuarioCancelacion",
                        column: x => x.IdUsuarioCancelacion,
                        principalSchema: "SistemaVIP",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Servicios_Presentadores_PresentadorId",
                        column: x => x.PresentadorId,
                        principalSchema: "SistemaVIP",
                        principalTable: "Presentadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TerapeutasPresentadores_Terapeutas_TerapeutaId",
                        column: x => x.TerapeutaId,
                        principalSchema: "SistemaVIP",
                        principalTable: "Terapeutas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    MontoTotal = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    MontoTerapeuta = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    MontoComisionTotal = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    MontoComisionEmpresa = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    MontoComisionPresentador = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    PorcentajeAplicadoEmpresa = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    PorcentajeAplicadoPresentador = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    FechaCalculo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NumeroTransaccion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ComprobanteUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaPagoTerapeuta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaLiquidacionPresentador = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NotasPago = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IdUsuarioConfirmacion = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FechaConfirmacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdUsuarioLiquidacion = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FechaLiquidacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comisiones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comisiones_AspNetUsers_IdUsuarioConfirmacion",
                        column: x => x.IdUsuarioConfirmacion,
                        principalSchema: "SistemaVIP",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comisiones_AspNetUsers_IdUsuarioLiquidacion",
                        column: x => x.IdUsuarioLiquidacion,
                        principalSchema: "SistemaVIP",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comisiones_Presentadores_PresentadorId",
                        column: x => x.PresentadorId,
                        principalSchema: "SistemaVIP",
                        principalTable: "Presentadores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comisiones_Servicios_ServicioId",
                        column: x => x.ServicioId,
                        principalSchema: "SistemaVIP",
                        principalTable: "Servicios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comisiones_Terapeutas_TerapeutaId",
                        column: x => x.TerapeutaId,
                        principalSchema: "SistemaVIP",
                        principalTable: "Terapeutas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ServiciosTerapeutas",
                schema: "SistemaVIP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServicioId = table.Column<int>(type: "int", nullable: false),
                    TerapeutaId = table.Column<int>(type: "int", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UbicacionInicio = table.Column<Point>(type: "geography", nullable: true),
                    UbicacionFin = table.Column<Point>(type: "geography", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MontoTerapeuta = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    GastosTransporte = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    NotasTransporte = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MontoEfectivo = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    MontoTransferencia = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    EvidenciaTransporte = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaRegistroGastosTransporte = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LinkConfirmacion = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LinkFinalizacion = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaComprobantePago = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UrlComprobantePago = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdPresentadorConfirmaPago = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NotasPago = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiciosTerapeutas", x => x.Id);
                    //table.CheckConstraint("CK_ServiciosTerapeutas_GastosTransporte", "([GastosTransporte] IS NULL AND EXISTS (SELECT 1 FROM Servicios s WHERE s.Id = ServicioId AND s.TipoUbicacion = 'CONSULTORIO')) OR ([GastosTransporte] IS NOT NULL AND EXISTS (SELECT 1 FROM Servicios s WHERE s.Id = ServicioId AND s.TipoUbicacion = 'DOMICILIO'))");
                    table.ForeignKey(
                        name: "FK_ServiciosTerapeutas_AspNetUsers_IdPresentadorConfirmaPago",
                        column: x => x.IdPresentadorConfirmaPago,
                        principalSchema: "SistemaVIP",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiciosTerapeutas_Servicios_ServicioId",
                        column: x => x.ServicioId,
                        principalSchema: "SistemaVIP",
                        principalTable: "Servicios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ServiciosTerapeutas_Terapeutas_TerapeutaId",
                        column: x => x.TerapeutaId,
                        principalSchema: "SistemaVIP",
                        principalTable: "Terapeutas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CambioEstadoServicioModel",
                schema: "SistemaVIP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServicioTerapeutaId = table.Column<int>(type: "int", nullable: false),
                    EstadoAnterior = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstadoNuevo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCambio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MotivosCambio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdUsuarioCambio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsuarioCambioId = table.Column<string>(type: "nvarchar(450)", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "ComprobantesPago",
                schema: "SistemaVIP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServicioTerapeutaId = table.Column<int>(type: "int", nullable: false),
                    TipoComprobante = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OrigenPago = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroOperacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UrlComprobante = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NotasComprobante = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IdUsuarioRegistro = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    MotivoRechazo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprobantesPago", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComprobantesPago_AspNetUsers_IdUsuarioRegistro",
                        column: x => x.IdUsuarioRegistro,
                        principalSchema: "SistemaVIP",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComprobantesPago_ServiciosTerapeutas_ServicioTerapeutaId",
                        column: x => x.ServicioTerapeutaId,
                        principalSchema: "SistemaVIP",
                        principalTable: "ServiciosTerapeutas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                schema: "SistemaVIP",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "SistemaVIP",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                schema: "SistemaVIP",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                schema: "SistemaVIP",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                schema: "SistemaVIP",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "SistemaVIP",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "SistemaVIP",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Bitacora_IdUsuario",
                schema: "SistemaVIP",
                table: "Bitacora",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Blacklist_IdUsuarioRegistro",
                schema: "SistemaVIP",
                table: "Blacklist",
                column: "IdUsuarioRegistro");

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

            migrationBuilder.CreateIndex(
                name: "IX_Comisiones_IdUsuarioConfirmacion",
                schema: "SistemaVIP",
                table: "Comisiones",
                column: "IdUsuarioConfirmacion");

            migrationBuilder.CreateIndex(
                name: "IX_Comisiones_IdUsuarioLiquidacion",
                schema: "SistemaVIP",
                table: "Comisiones",
                column: "IdUsuarioLiquidacion");

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
                name: "IX_ComprobantesPago_IdUsuarioRegistro",
                schema: "SistemaVIP",
                table: "ComprobantesPago",
                column: "IdUsuarioRegistro");

            migrationBuilder.CreateIndex(
                name: "IX_ComprobantesPago_NumeroOperacion",
                schema: "SistemaVIP",
                table: "ComprobantesPago",
                column: "NumeroOperacion",
                unique: true,
                filter: "[NumeroOperacion] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ComprobantesPago_ServicioTerapeutaId",
                schema: "SistemaVIP",
                table: "ComprobantesPago",
                column: "ServicioTerapeutaId");

            migrationBuilder.CreateIndex(
                name: "IX_Presentadores_UserId",
                schema: "SistemaVIP",
                table: "Presentadores",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_IdUsuarioCancelacion",
                schema: "SistemaVIP",
                table: "Servicios",
                column: "IdUsuarioCancelacion");

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_PresentadorId",
                schema: "SistemaVIP",
                table: "Servicios",
                column: "PresentadorId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosTerapeutas_IdPresentadorConfirmaPago",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                column: "IdPresentadorConfirmaPago");

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosTerapeutas_ServicioId_TerapeutaId",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                columns: new[] { "ServicioId", "TerapeutaId" },
                unique: true);

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
                name: "AspNetRoleClaims",
                schema: "SistemaVIP");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims",
                schema: "SistemaVIP");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins",
                schema: "SistemaVIP");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles",
                schema: "SistemaVIP");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens",
                schema: "SistemaVIP");

            migrationBuilder.DropTable(
                name: "Bitacora",
                schema: "SistemaVIP");

            migrationBuilder.DropTable(
                name: "Blacklist",
                schema: "SistemaVIP");

            migrationBuilder.DropTable(
                name: "CambioEstadoServicioModel",
                schema: "SistemaVIP");

            migrationBuilder.DropTable(
                name: "Comisiones",
                schema: "SistemaVIP");

            migrationBuilder.DropTable(
                name: "ComprobantesPago",
                schema: "SistemaVIP");

            migrationBuilder.DropTable(
                name: "TerapeutasPresentadores",
                schema: "SistemaVIP");

            migrationBuilder.DropTable(
                name: "AspNetRoles",
                schema: "SistemaVIP");

            migrationBuilder.DropTable(
                name: "ServiciosTerapeutas",
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

            migrationBuilder.DropTable(
                name: "AspNetUsers",
                schema: "SistemaVIP");
        }
    }
}
