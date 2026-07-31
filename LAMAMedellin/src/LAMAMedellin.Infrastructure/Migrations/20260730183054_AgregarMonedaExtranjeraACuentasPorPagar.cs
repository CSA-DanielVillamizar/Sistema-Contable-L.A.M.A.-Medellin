using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAMAMedellin.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarMonedaExtranjeraACuentasPorPagar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TasaCambioReconocida",
                table: "CuentasPorPagar",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorUSD",
                table: "CuentasPorPagar",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TasaCambioReconocida",
                table: "CuentasPorPagar");

            migrationBuilder.DropColumn(
                name: "ValorUSD",
                table: "CuentasPorPagar");
        }
    }
}
