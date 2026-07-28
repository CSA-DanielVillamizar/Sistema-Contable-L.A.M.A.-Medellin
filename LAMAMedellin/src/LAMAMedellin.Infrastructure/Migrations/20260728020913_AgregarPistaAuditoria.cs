using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAMAMedellin.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPistaAuditoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Usuarios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Usuarios",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Usuarios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Usuarios",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Usuarios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Usuarios",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Transacciones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Transacciones",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Transacciones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Transacciones",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Transacciones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Transacciones",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "TarifasCuota",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "TarifasCuota",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "TarifasCuota",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "TarifasCuota",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "TarifasCuota",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "TarifasCuota",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ProyectosSociales",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ProyectosSociales",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ProyectosSociales",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "ProyectosSociales",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ProyectosSociales",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ProyectosSociales",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Productos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Productos",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Productos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Productos",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Productos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Productos",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "MovimientosInventario",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "MovimientosInventario",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "MovimientosInventario",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "MovimientosInventario",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "MovimientosInventario",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "MovimientosInventario",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Miembros",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Miembros",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Miembros",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Miembros",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Miembros",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Miembros",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Ingresos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Ingresos",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Ingresos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Ingresos",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Ingresos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Ingresos",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Eventos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Eventos",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Eventos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Eventos",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Eventos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Eventos",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Egresos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Egresos",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Egresos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Egresos",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Egresos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Egresos",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Donantes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Donantes",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Donantes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Donantes",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Donantes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Donantes",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Donaciones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Donaciones",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Donaciones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Donaciones",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Donaciones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Donaciones",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CuotasAsamblea",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "CuotasAsamblea",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "CuotasAsamblea",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "CuotasAsamblea",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "CuotasAsamblea",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "CuotasAsamblea",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CuentasPorCobrar",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "CuentasPorCobrar",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "CuentasPorCobrar",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "CuentasPorCobrar",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "CuentasPorCobrar",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "CuentasPorCobrar",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CuentasContables",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "CuentasContables",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "CuentasContables",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "CuentasContables",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "CuentasContables",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "CuentasContables",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ConceptosCobro",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ConceptosCobro",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ConceptosCobro",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "ConceptosCobro",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ConceptosCobro",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ConceptosCobro",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Comprobantes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Comprobantes",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Comprobantes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Comprobantes",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Comprobantes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Comprobantes",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CentrosCosto",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "CentrosCosto",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "CentrosCosto",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "CentrosCosto",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "CentrosCosto",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "CentrosCosto",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Cajas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Cajas",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Cajas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Cajas",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Cajas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Cajas",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Beneficiarios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Beneficiarios",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Beneficiarios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Beneficiarios",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Beneficiarios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Beneficiarios",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Bancos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Bancos",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Bancos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Bancos",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Bancos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Bancos",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AsistenciasEvento",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "AsistenciasEvento",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "AsistenciasEvento",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "AsistenciasEvento",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AsistenciasEvento",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "AsistenciasEvento",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AsientosContables",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "AsientosContables",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "AsientosContables",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "AsientosContables",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AsientosContables",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "AsientosContables",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Transacciones");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Transacciones");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Transacciones");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Transacciones");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Transacciones");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Transacciones");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "TarifasCuota");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TarifasCuota");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "TarifasCuota");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "TarifasCuota");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "TarifasCuota");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "TarifasCuota");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ProyectosSociales");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ProyectosSociales");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ProyectosSociales");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ProyectosSociales");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ProyectosSociales");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ProyectosSociales");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "MovimientosInventario");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "MovimientosInventario");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "MovimientosInventario");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "MovimientosInventario");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "MovimientosInventario");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "MovimientosInventario");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Miembros");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Miembros");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Miembros");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Miembros");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Miembros");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Miembros");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Ingresos");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Ingresos");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Ingresos");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Ingresos");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Ingresos");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Ingresos");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Egresos");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Egresos");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Egresos");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Egresos");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Egresos");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Egresos");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Donantes");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Donantes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Donantes");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Donantes");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Donantes");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Donantes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Donaciones");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Donaciones");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Donaciones");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Donaciones");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Donaciones");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Donaciones");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CuotasAsamblea");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "CuotasAsamblea");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CuotasAsamblea");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "CuotasAsamblea");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "CuotasAsamblea");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "CuotasAsamblea");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CuentasPorCobrar");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "CuentasPorCobrar");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CuentasPorCobrar");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "CuentasPorCobrar");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "CuentasPorCobrar");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "CuentasPorCobrar");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CuentasContables");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "CuentasContables");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CuentasContables");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "CuentasContables");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "CuentasContables");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "CuentasContables");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ConceptosCobro");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ConceptosCobro");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ConceptosCobro");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ConceptosCobro");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ConceptosCobro");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ConceptosCobro");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Comprobantes");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Comprobantes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Comprobantes");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Comprobantes");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Comprobantes");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Comprobantes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CentrosCosto");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "CentrosCosto");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CentrosCosto");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "CentrosCosto");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "CentrosCosto");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "CentrosCosto");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Cajas");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Cajas");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Cajas");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Cajas");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Cajas");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Cajas");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Beneficiarios");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Beneficiarios");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Beneficiarios");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Beneficiarios");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Beneficiarios");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Beneficiarios");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Bancos");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Bancos");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Bancos");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Bancos");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Bancos");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Bancos");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AsistenciasEvento");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AsistenciasEvento");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "AsistenciasEvento");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "AsistenciasEvento");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AsistenciasEvento");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "AsistenciasEvento");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AsientosContables");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AsientosContables");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "AsientosContables");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "AsientosContables");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AsientosContables");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "AsientosContables");
        }
    }
}
