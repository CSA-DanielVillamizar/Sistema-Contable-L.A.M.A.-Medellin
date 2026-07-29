using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAMAMedellin.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarMedioPagoAMovimientos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MedioPago",
                table: "Ingresos",
                type: "int",
                nullable: false,
                // 1 = Transferencia. El andamiaje proponia 0, que no
                // corresponde a ningun miembro del enum y habria dejado las
                // filas existentes con un valor imposible de interpretar.
                // Transferencia es la lectura fiel de como opera la fundacion:
                // todo se recibe y se paga por transferencia bancaria.
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "MedioPago",
                table: "Egresos",
                type: "int",
                nullable: false,
                // 1 = Transferencia. El andamiaje proponia 0, que no
                // corresponde a ningun miembro del enum y habria dejado las
                // filas existentes con un valor imposible de interpretar.
                // Transferencia es la lectura fiel de como opera la fundacion:
                // todo se recibe y se paga por transferencia bancaria.
                defaultValue: 1);
        }

        /// <inheritdoc />
        private static void NormalizarFilasExistentes(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Ingresos SET MedioPago = 1 WHERE MedioPago NOT BETWEEN 1 AND 4;");
            migrationBuilder.Sql("UPDATE Egresos  SET MedioPago = 1 WHERE MedioPago NOT BETWEEN 1 AND 4;");

            NormalizarFilasExistentes(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MedioPago",
                table: "Ingresos");

            migrationBuilder.DropColumn(
                name: "MedioPago",
                table: "Egresos");
        }
    }
}
