using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAMAMedellin.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AplicarCuotasLogisticaYRenovacionMembresia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Estas columnas las introdujo 20260728191700_AgregarCuotasLogisticaY
            // RenovacionMembresia, pero esa migracion quedo sin su archivo
            // .Designer.cs porque .gitignore excluia *.Designer.cs. Sin ese
            // archivo no lleva el atributo [Migration], EF no la reconoce y la
            // salta en silencio informando que la base "ya esta al dia".
            //
            // Esta migracion las crea de verdad. Se comprueba la existencia
            // porque en bases donde aquella si alcanzo a aplicarse ya estarian.
            migrationBuilder.Sql(@"
                IF COL_LENGTH('CuotasAsamblea', 'RenovacionMembresiaUSD') IS NULL
                    ALTER TABLE CuotasAsamblea ADD RenovacionMembresiaUSD decimal(18,2) NULL;
            ");

            migrationBuilder.Sql(@"
                IF COL_LENGTH('Eventos', 'CuotaLogisticaCOP') IS NULL
                    ALTER TABLE Eventos ADD CuotaLogisticaCOP decimal(18,2) NOT NULL DEFAULT 0;
            ");

            migrationBuilder.Sql(@"
                IF COL_LENGTH('AsistenciasEvento', 'CuotaAplicadaCOP') IS NULL
                    ALTER TABLE AsistenciasEvento ADD CuotaAplicadaCOP decimal(18,2) NOT NULL DEFAULT 0;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("IF COL_LENGTH('CuotasAsamblea','RenovacionMembresiaUSD') IS NOT NULL ALTER TABLE CuotasAsamblea DROP COLUMN RenovacionMembresiaUSD;");
            migrationBuilder.Sql("IF COL_LENGTH('Eventos','CuotaLogisticaCOP') IS NOT NULL ALTER TABLE Eventos DROP COLUMN CuotaLogisticaCOP;");
            migrationBuilder.Sql("IF COL_LENGTH('AsistenciasEvento','CuotaAplicadaCOP') IS NOT NULL ALTER TABLE AsistenciasEvento DROP COLUMN CuotaAplicadaCOP;");
        }
    }
}
