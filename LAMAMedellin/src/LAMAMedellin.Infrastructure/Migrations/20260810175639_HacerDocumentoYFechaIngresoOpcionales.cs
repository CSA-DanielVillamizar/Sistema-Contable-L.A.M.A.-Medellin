using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAMAMedellin.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HacerDocumentoYFechaIngresoOpcionales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Miembros_DocumentoIdentidad",
                table: "Miembros");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "FechaIngreso",
                table: "Miembros",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "DocumentoIdentidad",
                table: "Miembros",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "IX_Miembros_DocumentoIdentidad",
                table: "Miembros",
                column: "DocumentoIdentidad",
                unique: true,
                filter: "[DocumentoIdentidad] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Miembros_DocumentoIdentidad",
                table: "Miembros");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "FechaIngreso",
                table: "Miembros",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DocumentoIdentidad",
                table: "Miembros",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Miembros_DocumentoIdentidad",
                table: "Miembros",
                column: "DocumentoIdentidad",
                unique: true);
        }
    }
}
