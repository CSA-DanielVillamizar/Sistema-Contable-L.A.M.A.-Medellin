using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAMAMedellin.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CarteraPorPeriodoYTipoAfiliacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CuentasPorCobrar_MiembroConceptoFecha",
                table: "CuentasPorCobrar");

            migrationBuilder.AddColumn<int>(
                name: "TipoAfiliacion",
                table: "Miembros",
                type: "int",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.AddColumn<string>(
                name: "Periodo",
                table: "CuentasPorCobrar",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: false,
                defaultValue: "");

                        // Las filas existentes no tenian periodo. Se deriva de la fecha de
            // emision, que es el dato mas fiel disponible; dejarlas en cadena
            // vacia habria producido obligaciones sin mes identificable.
            migrationBuilder.Sql(
                "UPDATE CuentasPorCobrar SET Periodo = FORMAT(FechaEmision, 'yyyy-MM') WHERE Periodo IS NULL OR Periodo = '';");

migrationBuilder.CreateIndex(
                name: "IX_CuentasPorCobrar_MiembroConceptoPeriodo",
                table: "CuentasPorCobrar",
                columns: new[] { "MiembroId", "ConceptoCobroId", "Periodo" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CuentasPorCobrar_MiembroConceptoPeriodo",
                table: "CuentasPorCobrar");

            migrationBuilder.DropColumn(
                name: "TipoAfiliacion",
                table: "Miembros");

            migrationBuilder.DropColumn(
                name: "Periodo",
                table: "CuentasPorCobrar");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasPorCobrar_MiembroConceptoFecha",
                table: "CuentasPorCobrar",
                columns: new[] { "MiembroId", "ConceptoCobroId", "FechaEmision" },
                unique: true);
        }
    }
}
