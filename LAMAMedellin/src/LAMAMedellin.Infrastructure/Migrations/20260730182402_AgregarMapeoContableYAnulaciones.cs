using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAMAMedellin.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarMapeoContableYAnulaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MapeosContables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoOperacion = table.Column<int>(type: "int", nullable: false),
                    CuentaContableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_MapeosContables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MapeosContables_CuentasContables_CuentaContableId",
                        column: x => x.CuentaContableId,
                        principalTable: "CuentasContables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesAnulacion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComprobanteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MotivoSolicitud = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    MotivoResolucion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ResueltaPor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FechaResolucion = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_SolicitudesAnulacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesAnulacion_Comprobantes_ComprobanteId",
                        column: x => x.ComprobanteId,
                        principalTable: "Comprobantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MapeosContables_CuentaContableId",
                table: "MapeosContables",
                column: "CuentaContableId");

            migrationBuilder.CreateIndex(
                name: "IX_MapeosContables_TipoOperacion",
                table: "MapeosContables",
                column: "TipoOperacion",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesAnulacion_ComprobanteId",
                table: "SolicitudesAnulacion",
                column: "ComprobanteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MapeosContables");

            migrationBuilder.DropTable(
                name: "SolicitudesAnulacion");
        }
    }
}
