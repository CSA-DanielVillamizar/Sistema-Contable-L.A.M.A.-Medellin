using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAMAMedellin.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCampanasDonacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TransaccionMultimoneda_ReferenciaSoporte",
                table: "Transacciones",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CampanaDonacionId",
                table: "Donaciones",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CampanasDonacion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    MetaCOP = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaFin = table.Column<DateOnly>(type: "date", nullable: false),
                    EstaActiva = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampanasDonacion", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Donaciones_CampanaDonacionId",
                table: "Donaciones",
                column: "CampanaDonacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Donaciones_CampanasDonacion_CampanaDonacionId",
                table: "Donaciones",
                column: "CampanaDonacionId",
                principalTable: "CampanasDonacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Donaciones_CampanasDonacion_CampanaDonacionId",
                table: "Donaciones");

            migrationBuilder.DropTable(
                name: "CampanasDonacion");

            migrationBuilder.DropIndex(
                name: "IX_Donaciones_CampanaDonacionId",
                table: "Donaciones");

            migrationBuilder.DropColumn(
                name: "TransaccionMultimoneda_ReferenciaSoporte",
                table: "Transacciones");

            migrationBuilder.DropColumn(
                name: "CampanaDonacionId",
                table: "Donaciones");
        }
    }
}
