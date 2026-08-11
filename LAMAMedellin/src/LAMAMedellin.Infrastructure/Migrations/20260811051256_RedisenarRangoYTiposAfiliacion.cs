using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAMAMedellin.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RedisenarRangoYTiposAfiliacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Rango",
                table: "Miembros",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

            // RangoClub paso de ser un nivel generico (Aspirante/Prospecto/
            // MiembroActivo/Directivo) a representar cargos reales de la
            // directiva (Presidente, Secretario, etc.). Los valores que ya
            // habia en Rango pertenecen al enum anterior y no corresponden a
            // ningun cargo real: casi todos quedaron en 1 (Aspirante) por ser
            // el valor por defecto del formulario, no porque alguien haya
            // asignado ese cargo. Se limpian en vez de reinterpretarlos bajo
            // el enum nuevo, siguiendo la misma regla del resto del sistema
            // de no inventar datos que no se conocen realmente.
            migrationBuilder.Sql("UPDATE Miembros SET Rango = NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Rango",
                table: "Miembros",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
