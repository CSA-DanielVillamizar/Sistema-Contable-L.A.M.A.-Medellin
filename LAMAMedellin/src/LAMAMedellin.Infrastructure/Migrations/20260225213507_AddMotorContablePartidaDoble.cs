using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAMAMedellin.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMotorContablePartidaDoble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comprobantes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumeroConsecutivo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoComprobante = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EstadoComprobante = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comprobantes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CuentasContables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    PermiteMovimiento = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentasContables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AsientosContables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComprobanteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CuentaContableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TerceroId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CentroCostoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Debe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Haber = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Referencia = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsientosContables", x => x.Id);
                    table.CheckConstraint("CK_AsientoContable_DebeHaber_Exclusivo", "(([Debe] > 0 AND [Haber] = 0) OR ([Debe] = 0 AND [Haber] > 0))");
                    table.ForeignKey(
                        name: "FK_AsientosContables_CentrosCosto_CentroCostoId",
                        column: x => x.CentroCostoId,
                        principalTable: "CentrosCosto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AsientosContables_Comprobantes_ComprobanteId",
                        column: x => x.ComprobanteId,
                        principalTable: "Comprobantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AsientosContables_CuentasContables_CuentaContableId",
                        column: x => x.CuentaContableId,
                        principalTable: "CuentasContables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AsientosContables_CentroCostoId",
                table: "AsientosContables",
                column: "CentroCostoId");

            migrationBuilder.CreateIndex(
                name: "IX_AsientosContables_ComprobanteId",
                table: "AsientosContables",
                column: "ComprobanteId");

            migrationBuilder.CreateIndex(
                name: "IX_AsientosContables_CuentaContableId",
                table: "AsientosContables",
                column: "CuentaContableId");

            migrationBuilder.CreateIndex(
                name: "IX_Comprobantes_NumeroConsecutivo",
                table: "Comprobantes",
                column: "NumeroConsecutivo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CuentasContables_Codigo",
                table: "CuentasContables",
                column: "Codigo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsientosContables");

            migrationBuilder.DropTable(
                name: "Comprobantes");

            migrationBuilder.DropTable(
                name: "CuentasContables");
        }
    }
}
