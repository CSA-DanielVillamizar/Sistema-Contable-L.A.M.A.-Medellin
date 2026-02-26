using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAMAMedellin.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDonacionesModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Donantes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NombreORazonSocial = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TipoDocumento = table.Column<int>(type: "int", nullable: false),
                    NumeroDocumento = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TipoPersona = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Donantes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Donaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonanteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MontoCOP = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BancoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CentroCostoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CertificadoEmitido = table.Column<bool>(type: "bit", nullable: false),
                    CodigoVerificacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Donaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Donaciones_Bancos_BancoId",
                        column: x => x.BancoId,
                        principalTable: "Bancos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Donaciones_CentrosCosto_CentroCostoId",
                        column: x => x.CentroCostoId,
                        principalTable: "CentrosCosto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Donaciones_Donantes_DonanteId",
                        column: x => x.DonanteId,
                        principalTable: "Donantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Donaciones_BancoId",
                table: "Donaciones",
                column: "BancoId");

            migrationBuilder.CreateIndex(
                name: "IX_Donaciones_CentroCostoId",
                table: "Donaciones",
                column: "CentroCostoId");

            migrationBuilder.CreateIndex(
                name: "IX_Donaciones_CodigoVerificacion",
                table: "Donaciones",
                column: "CodigoVerificacion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Donaciones_DonanteId",
                table: "Donaciones",
                column: "DonanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Donantes_TipoDocumento_NumeroDocumento",
                table: "Donantes",
                columns: new[] { "TipoDocumento", "NumeroDocumento" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Donaciones");

            migrationBuilder.DropTable(
                name: "Donantes");
        }
    }
}
