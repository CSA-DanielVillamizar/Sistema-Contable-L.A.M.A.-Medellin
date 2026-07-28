using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAMAMedellin.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCuotasLogisticaYRenovacionMembresia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Cuota de renovación de membresía internacional en USD (CuotasAsamblea)
            migrationBuilder.AddColumn<decimal>(
                name: "RenovacionMembresiaUSD",
                table: "CuotasAsamblea",
                type: "decimal(18,4)",
                nullable: true);

            // Cuota logística por asistente al evento (Eventos)
            migrationBuilder.AddColumn<decimal>(
                name: "CuotaLogisticaCOP",
                table: "Eventos",
                type: "decimal(18,2)",
                nullable: true);

            // Snapshot de la cuota aplicada al momento del registro (AsistenciasEvento)
            migrationBuilder.AddColumn<decimal>(
                name: "CuotaAplicadaCOP",
                table: "AsistenciasEvento",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CuotaAplicadaCOP",
                table: "AsistenciasEvento");

            migrationBuilder.DropColumn(
                name: "CuotaLogisticaCOP",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "RenovacionMembresiaUSD",
                table: "CuotasAsamblea");
        }
    }
}
