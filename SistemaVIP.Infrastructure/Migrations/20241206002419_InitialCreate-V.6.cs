using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaVIP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateV6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bitacora_AspNetUsers_UsuarioId",
                schema: "SistemaVIP",
                table: "Bitacora");

            migrationBuilder.DropForeignKey(
                name: "FK_Blacklist_AspNetUsers_UsuarioRegistroId",
                schema: "SistemaVIP",
                table: "Blacklist");

            migrationBuilder.DropForeignKey(
                name: "FK_Comisiones_Presentadores_PresentadorId",
                schema: "SistemaVIP",
                table: "Comisiones");

            migrationBuilder.DropForeignKey(
                name: "FK_Comisiones_Servicios_ServicioId",
                schema: "SistemaVIP",
                table: "Comisiones");

            migrationBuilder.DropForeignKey(
                name: "FK_Comisiones_Terapeutas_TerapeutaId",
                schema: "SistemaVIP",
                table: "Comisiones");

            migrationBuilder.DropForeignKey(
                name: "FK_ConfiguracionComisiones_AspNetUsers_UserId",
                schema: "SistemaVIP",
                table: "ConfiguracionComisiones");

            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_AspNetUsers_UsuarioRegistroId",
                schema: "SistemaVIP",
                table: "Pagos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_AspNetUsers_UsuarioValidacionId",
                schema: "SistemaVIP",
                table: "Pagos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_Servicios_ServicioId",
                schema: "SistemaVIP",
                table: "Pagos");

            migrationBuilder.DropForeignKey(
                name: "FK_Presentadores_AspNetUsers_UserId",
                schema: "SistemaVIP",
                table: "Presentadores");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicios_AspNetUsers_UsuarioCancelacionId",
                schema: "SistemaVIP",
                table: "Servicios");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicios_Presentadores_PresentadorId",
                schema: "SistemaVIP",
                table: "Servicios");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiciosTerapeutas_AspNetUsers_PresentadorConfirmaPagoId",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiciosTerapeutas_Servicios_ServicioId",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiciosTerapeutas_Terapeutas_TerapeutaId",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas");

            migrationBuilder.DropForeignKey(
                name: "FK_Terapeutas_AspNetUsers_UserId",
                schema: "SistemaVIP",
                table: "Terapeutas");

            migrationBuilder.DropForeignKey(
                name: "FK_TerapeutasPresentadores_Presentadores_PresentadorId",
                schema: "SistemaVIP",
                table: "TerapeutasPresentadores");

            migrationBuilder.DropForeignKey(
                name: "FK_TerapeutasPresentadores_Terapeutas_TerapeutaId",
                schema: "SistemaVIP",
                table: "TerapeutasPresentadores");

            migrationBuilder.DropIndex(
                name: "IX_ServiciosTerapeutas_PresentadorConfirmaPagoId",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas");

            migrationBuilder.DropIndex(
                name: "IX_Servicios_UsuarioCancelacionId",
                schema: "SistemaVIP",
                table: "Servicios");

            migrationBuilder.DropIndex(
                name: "IX_Pagos_UsuarioRegistroId",
                schema: "SistemaVIP",
                table: "Pagos");

            migrationBuilder.DropIndex(
                name: "IX_Pagos_UsuarioValidacionId",
                schema: "SistemaVIP",
                table: "Pagos");

            migrationBuilder.DropIndex(
                name: "IX_ConfiguracionComisiones_UserId",
                schema: "SistemaVIP",
                table: "ConfiguracionComisiones");

            migrationBuilder.DropIndex(
                name: "IX_Blacklist_UsuarioRegistroId",
                schema: "SistemaVIP",
                table: "Blacklist");

            migrationBuilder.DropIndex(
                name: "IX_Bitacora_UsuarioId",
                schema: "SistemaVIP",
                table: "Bitacora");

            migrationBuilder.DropColumn(
                name: "PresentadorConfirmaPagoId",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas");

            migrationBuilder.DropColumn(
                name: "UsuarioCancelacionId",
                schema: "SistemaVIP",
                table: "Servicios");

            migrationBuilder.DropColumn(
                name: "UsuarioRegistroId",
                schema: "SistemaVIP",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "UsuarioValidacionId",
                schema: "SistemaVIP",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "SistemaVIP",
                table: "ConfiguracionComisiones");

            migrationBuilder.DropColumn(
                name: "UsuarioRegistroId",
                schema: "SistemaVIP",
                table: "Blacklist");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                schema: "SistemaVIP",
                table: "Bitacora");

            migrationBuilder.AlterColumn<decimal>(
                name: "TarifaExtra",
                schema: "SistemaVIP",
                table: "Terapeutas",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TarifaBase",
                schema: "SistemaVIP",
                table: "Terapeutas",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCambioEstado",
                schema: "SistemaVIP",
                table: "Terapeutas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "MotivoEstado",
                schema: "SistemaVIP",
                table: "Terapeutas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MontoTerapeuta",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IdPresentadorConfirmaPago",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MontoTotal",
                schema: "SistemaVIP",
                table: "Servicios",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "IdUsuarioCancelacion",
                schema: "SistemaVIP",
                table: "Servicios",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PorcentajeComision",
                schema: "SistemaVIP",
                table: "Presentadores",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCambioEstado",
                schema: "SistemaVIP",
                table: "Presentadores",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "MotivoEstado",
                schema: "SistemaVIP",
                table: "Presentadores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Monto",
                schema: "SistemaVIP",
                table: "Pagos",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "IdUsuarioValidacion",
                schema: "SistemaVIP",
                table: "Pagos",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IdUsuarioRegistro",
                schema: "SistemaVIP",
                table: "Pagos",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PorcentajePresentador",
                schema: "SistemaVIP",
                table: "ConfiguracionComisiones",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PorcentajeEmpresa",
                schema: "SistemaVIP",
                table: "ConfiguracionComisiones",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "CreadoPor",
                schema: "SistemaVIP",
                table: "ConfiguracionComisiones",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PorcentajeAplicadoPresentador",
                schema: "SistemaVIP",
                table: "Comisiones",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PorcentajeAplicadoEmpresa",
                schema: "SistemaVIP",
                table: "Comisiones",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MontoTotal",
                schema: "SistemaVIP",
                table: "Comisiones",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MontoTerapeuta",
                schema: "SistemaVIP",
                table: "Comisiones",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MontoComisionTotal",
                schema: "SistemaVIP",
                table: "Comisiones",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MontoComisionPresentador",
                schema: "SistemaVIP",
                table: "Comisiones",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MontoComisionEmpresa",
                schema: "SistemaVIP",
                table: "Comisiones",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "IdUsuarioRegistro",
                schema: "SistemaVIP",
                table: "Blacklist",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "IdUsuario",
                schema: "SistemaVIP",
                table: "Bitacora",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosTerapeutas_IdPresentadorConfirmaPago",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                column: "IdPresentadorConfirmaPago");

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_IdUsuarioCancelacion",
                schema: "SistemaVIP",
                table: "Servicios",
                column: "IdUsuarioCancelacion");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_IdUsuarioRegistro",
                schema: "SistemaVIP",
                table: "Pagos",
                column: "IdUsuarioRegistro");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_IdUsuarioValidacion",
                schema: "SistemaVIP",
                table: "Pagos",
                column: "IdUsuarioValidacion");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionComisiones_CreadoPor",
                schema: "SistemaVIP",
                table: "ConfiguracionComisiones",
                column: "CreadoPor");

            migrationBuilder.CreateIndex(
                name: "IX_Blacklist_IdUsuarioRegistro",
                schema: "SistemaVIP",
                table: "Blacklist",
                column: "IdUsuarioRegistro");

            migrationBuilder.CreateIndex(
                name: "IX_Bitacora_IdUsuario",
                schema: "SistemaVIP",
                table: "Bitacora",
                column: "IdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Bitacora_AspNetUsers_IdUsuario",
                schema: "SistemaVIP",
                table: "Bitacora",
                column: "IdUsuario",
                principalSchema: "SistemaVIP",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Blacklist_AspNetUsers_IdUsuarioRegistro",
                schema: "SistemaVIP",
                table: "Blacklist",
                column: "IdUsuarioRegistro",
                principalSchema: "SistemaVIP",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comisiones_Presentadores_PresentadorId",
                schema: "SistemaVIP",
                table: "Comisiones",
                column: "PresentadorId",
                principalSchema: "SistemaVIP",
                principalTable: "Presentadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comisiones_Servicios_ServicioId",
                schema: "SistemaVIP",
                table: "Comisiones",
                column: "ServicioId",
                principalSchema: "SistemaVIP",
                principalTable: "Servicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comisiones_Terapeutas_TerapeutaId",
                schema: "SistemaVIP",
                table: "Comisiones",
                column: "TerapeutaId",
                principalSchema: "SistemaVIP",
                principalTable: "Terapeutas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ConfiguracionComisiones_AspNetUsers_CreadoPor",
                schema: "SistemaVIP",
                table: "ConfiguracionComisiones",
                column: "CreadoPor",
                principalSchema: "SistemaVIP",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_AspNetUsers_IdUsuarioRegistro",
                schema: "SistemaVIP",
                table: "Pagos",
                column: "IdUsuarioRegistro",
                principalSchema: "SistemaVIP",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_AspNetUsers_IdUsuarioValidacion",
                schema: "SistemaVIP",
                table: "Pagos",
                column: "IdUsuarioValidacion",
                principalSchema: "SistemaVIP",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_Servicios_ServicioId",
                schema: "SistemaVIP",
                table: "Pagos",
                column: "ServicioId",
                principalSchema: "SistemaVIP",
                principalTable: "Servicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Presentadores_AspNetUsers_UserId",
                schema: "SistemaVIP",
                table: "Presentadores",
                column: "UserId",
                principalSchema: "SistemaVIP",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Servicios_AspNetUsers_IdUsuarioCancelacion",
                schema: "SistemaVIP",
                table: "Servicios",
                column: "IdUsuarioCancelacion",
                principalSchema: "SistemaVIP",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Servicios_Presentadores_PresentadorId",
                schema: "SistemaVIP",
                table: "Servicios",
                column: "PresentadorId",
                principalSchema: "SistemaVIP",
                principalTable: "Presentadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiciosTerapeutas_AspNetUsers_IdPresentadorConfirmaPago",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                column: "IdPresentadorConfirmaPago",
                principalSchema: "SistemaVIP",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiciosTerapeutas_Servicios_ServicioId",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                column: "ServicioId",
                principalSchema: "SistemaVIP",
                principalTable: "Servicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiciosTerapeutas_Terapeutas_TerapeutaId",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                column: "TerapeutaId",
                principalSchema: "SistemaVIP",
                principalTable: "Terapeutas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Terapeutas_AspNetUsers_UserId",
                schema: "SistemaVIP",
                table: "Terapeutas",
                column: "UserId",
                principalSchema: "SistemaVIP",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TerapeutasPresentadores_Presentadores_PresentadorId",
                schema: "SistemaVIP",
                table: "TerapeutasPresentadores",
                column: "PresentadorId",
                principalSchema: "SistemaVIP",
                principalTable: "Presentadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TerapeutasPresentadores_Terapeutas_TerapeutaId",
                schema: "SistemaVIP",
                table: "TerapeutasPresentadores",
                column: "TerapeutaId",
                principalSchema: "SistemaVIP",
                principalTable: "Terapeutas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bitacora_AspNetUsers_IdUsuario",
                schema: "SistemaVIP",
                table: "Bitacora");

            migrationBuilder.DropForeignKey(
                name: "FK_Blacklist_AspNetUsers_IdUsuarioRegistro",
                schema: "SistemaVIP",
                table: "Blacklist");

            migrationBuilder.DropForeignKey(
                name: "FK_Comisiones_Presentadores_PresentadorId",
                schema: "SistemaVIP",
                table: "Comisiones");

            migrationBuilder.DropForeignKey(
                name: "FK_Comisiones_Servicios_ServicioId",
                schema: "SistemaVIP",
                table: "Comisiones");

            migrationBuilder.DropForeignKey(
                name: "FK_Comisiones_Terapeutas_TerapeutaId",
                schema: "SistemaVIP",
                table: "Comisiones");

            migrationBuilder.DropForeignKey(
                name: "FK_ConfiguracionComisiones_AspNetUsers_CreadoPor",
                schema: "SistemaVIP",
                table: "ConfiguracionComisiones");

            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_AspNetUsers_IdUsuarioRegistro",
                schema: "SistemaVIP",
                table: "Pagos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_AspNetUsers_IdUsuarioValidacion",
                schema: "SistemaVIP",
                table: "Pagos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_Servicios_ServicioId",
                schema: "SistemaVIP",
                table: "Pagos");

            migrationBuilder.DropForeignKey(
                name: "FK_Presentadores_AspNetUsers_UserId",
                schema: "SistemaVIP",
                table: "Presentadores");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicios_AspNetUsers_IdUsuarioCancelacion",
                schema: "SistemaVIP",
                table: "Servicios");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicios_Presentadores_PresentadorId",
                schema: "SistemaVIP",
                table: "Servicios");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiciosTerapeutas_AspNetUsers_IdPresentadorConfirmaPago",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiciosTerapeutas_Servicios_ServicioId",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiciosTerapeutas_Terapeutas_TerapeutaId",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas");

            migrationBuilder.DropForeignKey(
                name: "FK_Terapeutas_AspNetUsers_UserId",
                schema: "SistemaVIP",
                table: "Terapeutas");

            migrationBuilder.DropForeignKey(
                name: "FK_TerapeutasPresentadores_Presentadores_PresentadorId",
                schema: "SistemaVIP",
                table: "TerapeutasPresentadores");

            migrationBuilder.DropForeignKey(
                name: "FK_TerapeutasPresentadores_Terapeutas_TerapeutaId",
                schema: "SistemaVIP",
                table: "TerapeutasPresentadores");

            migrationBuilder.DropIndex(
                name: "IX_ServiciosTerapeutas_IdPresentadorConfirmaPago",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas");

            migrationBuilder.DropIndex(
                name: "IX_Servicios_IdUsuarioCancelacion",
                schema: "SistemaVIP",
                table: "Servicios");

            migrationBuilder.DropIndex(
                name: "IX_Pagos_IdUsuarioRegistro",
                schema: "SistemaVIP",
                table: "Pagos");

            migrationBuilder.DropIndex(
                name: "IX_Pagos_IdUsuarioValidacion",
                schema: "SistemaVIP",
                table: "Pagos");

            migrationBuilder.DropIndex(
                name: "IX_ConfiguracionComisiones_CreadoPor",
                schema: "SistemaVIP",
                table: "ConfiguracionComisiones");

            migrationBuilder.DropIndex(
                name: "IX_Blacklist_IdUsuarioRegistro",
                schema: "SistemaVIP",
                table: "Blacklist");

            migrationBuilder.DropIndex(
                name: "IX_Bitacora_IdUsuario",
                schema: "SistemaVIP",
                table: "Bitacora");

            migrationBuilder.DropColumn(
                name: "FechaCambioEstado",
                schema: "SistemaVIP",
                table: "Terapeutas");

            migrationBuilder.DropColumn(
                name: "MotivoEstado",
                schema: "SistemaVIP",
                table: "Terapeutas");

            migrationBuilder.DropColumn(
                name: "FechaCambioEstado",
                schema: "SistemaVIP",
                table: "Presentadores");

            migrationBuilder.DropColumn(
                name: "MotivoEstado",
                schema: "SistemaVIP",
                table: "Presentadores");

            migrationBuilder.AlterColumn<decimal>(
                name: "TarifaExtra",
                schema: "SistemaVIP",
                table: "Terapeutas",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "TarifaBase",
                schema: "SistemaVIP",
                table: "Terapeutas",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "MontoTerapeuta",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IdPresentadorConfirmaPago",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PresentadorConfirmaPagoId",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MontoTotal",
                schema: "SistemaVIP",
                table: "Servicios",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "IdUsuarioCancelacion",
                schema: "SistemaVIP",
                table: "Servicios",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioCancelacionId",
                schema: "SistemaVIP",
                table: "Servicios",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PorcentajeComision",
                schema: "SistemaVIP",
                table: "Presentadores",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "Monto",
                schema: "SistemaVIP",
                table: "Pagos",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "IdUsuarioValidacion",
                schema: "SistemaVIP",
                table: "Pagos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IdUsuarioRegistro",
                schema: "SistemaVIP",
                table: "Pagos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioRegistroId",
                schema: "SistemaVIP",
                table: "Pagos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioValidacionId",
                schema: "SistemaVIP",
                table: "Pagos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PorcentajePresentador",
                schema: "SistemaVIP",
                table: "ConfiguracionComisiones",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "PorcentajeEmpresa",
                schema: "SistemaVIP",
                table: "ConfiguracionComisiones",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "CreadoPor",
                schema: "SistemaVIP",
                table: "ConfiguracionComisiones",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                schema: "SistemaVIP",
                table: "ConfiguracionComisiones",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PorcentajeAplicadoPresentador",
                schema: "SistemaVIP",
                table: "Comisiones",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "PorcentajeAplicadoEmpresa",
                schema: "SistemaVIP",
                table: "Comisiones",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "MontoTotal",
                schema: "SistemaVIP",
                table: "Comisiones",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "MontoTerapeuta",
                schema: "SistemaVIP",
                table: "Comisiones",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "MontoComisionTotal",
                schema: "SistemaVIP",
                table: "Comisiones",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "MontoComisionPresentador",
                schema: "SistemaVIP",
                table: "Comisiones",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "MontoComisionEmpresa",
                schema: "SistemaVIP",
                table: "Comisiones",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "IdUsuarioRegistro",
                schema: "SistemaVIP",
                table: "Blacklist",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioRegistroId",
                schema: "SistemaVIP",
                table: "Blacklist",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IdUsuario",
                schema: "SistemaVIP",
                table: "Bitacora",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                schema: "SistemaVIP",
                table: "Bitacora",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosTerapeutas_PresentadorConfirmaPagoId",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                column: "PresentadorConfirmaPagoId");

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_UsuarioCancelacionId",
                schema: "SistemaVIP",
                table: "Servicios",
                column: "UsuarioCancelacionId");

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
                name: "IX_ConfiguracionComisiones_UserId",
                schema: "SistemaVIP",
                table: "ConfiguracionComisiones",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Blacklist_UsuarioRegistroId",
                schema: "SistemaVIP",
                table: "Blacklist",
                column: "UsuarioRegistroId");

            migrationBuilder.CreateIndex(
                name: "IX_Bitacora_UsuarioId",
                schema: "SistemaVIP",
                table: "Bitacora",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bitacora_AspNetUsers_UsuarioId",
                schema: "SistemaVIP",
                table: "Bitacora",
                column: "UsuarioId",
                principalSchema: "SistemaVIP",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Blacklist_AspNetUsers_UsuarioRegistroId",
                schema: "SistemaVIP",
                table: "Blacklist",
                column: "UsuarioRegistroId",
                principalSchema: "SistemaVIP",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comisiones_Presentadores_PresentadorId",
                schema: "SistemaVIP",
                table: "Comisiones",
                column: "PresentadorId",
                principalSchema: "SistemaVIP",
                principalTable: "Presentadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comisiones_Servicios_ServicioId",
                schema: "SistemaVIP",
                table: "Comisiones",
                column: "ServicioId",
                principalSchema: "SistemaVIP",
                principalTable: "Servicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comisiones_Terapeutas_TerapeutaId",
                schema: "SistemaVIP",
                table: "Comisiones",
                column: "TerapeutaId",
                principalSchema: "SistemaVIP",
                principalTable: "Terapeutas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConfiguracionComisiones_AspNetUsers_UserId",
                schema: "SistemaVIP",
                table: "ConfiguracionComisiones",
                column: "UserId",
                principalSchema: "SistemaVIP",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_AspNetUsers_UsuarioRegistroId",
                schema: "SistemaVIP",
                table: "Pagos",
                column: "UsuarioRegistroId",
                principalSchema: "SistemaVIP",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_AspNetUsers_UsuarioValidacionId",
                schema: "SistemaVIP",
                table: "Pagos",
                column: "UsuarioValidacionId",
                principalSchema: "SistemaVIP",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_Servicios_ServicioId",
                schema: "SistemaVIP",
                table: "Pagos",
                column: "ServicioId",
                principalSchema: "SistemaVIP",
                principalTable: "Servicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Presentadores_AspNetUsers_UserId",
                schema: "SistemaVIP",
                table: "Presentadores",
                column: "UserId",
                principalSchema: "SistemaVIP",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Servicios_AspNetUsers_UsuarioCancelacionId",
                schema: "SistemaVIP",
                table: "Servicios",
                column: "UsuarioCancelacionId",
                principalSchema: "SistemaVIP",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Servicios_Presentadores_PresentadorId",
                schema: "SistemaVIP",
                table: "Servicios",
                column: "PresentadorId",
                principalSchema: "SistemaVIP",
                principalTable: "Presentadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiciosTerapeutas_AspNetUsers_PresentadorConfirmaPagoId",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                column: "PresentadorConfirmaPagoId",
                principalSchema: "SistemaVIP",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiciosTerapeutas_Servicios_ServicioId",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                column: "ServicioId",
                principalSchema: "SistemaVIP",
                principalTable: "Servicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiciosTerapeutas_Terapeutas_TerapeutaId",
                schema: "SistemaVIP",
                table: "ServiciosTerapeutas",
                column: "TerapeutaId",
                principalSchema: "SistemaVIP",
                principalTable: "Terapeutas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Terapeutas_AspNetUsers_UserId",
                schema: "SistemaVIP",
                table: "Terapeutas",
                column: "UserId",
                principalSchema: "SistemaVIP",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TerapeutasPresentadores_Presentadores_PresentadorId",
                schema: "SistemaVIP",
                table: "TerapeutasPresentadores",
                column: "PresentadorId",
                principalSchema: "SistemaVIP",
                principalTable: "Presentadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TerapeutasPresentadores_Terapeutas_TerapeutaId",
                schema: "SistemaVIP",
                table: "TerapeutasPresentadores",
                column: "TerapeutaId",
                principalSchema: "SistemaVIP",
                principalTable: "Terapeutas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
