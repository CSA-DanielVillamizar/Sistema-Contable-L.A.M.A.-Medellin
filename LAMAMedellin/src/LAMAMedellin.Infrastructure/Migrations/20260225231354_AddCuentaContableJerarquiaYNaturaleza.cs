using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAMAMedellin.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCuentaContableJerarquiaYNaturaleza : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CuentaPadreId",
                table: "CuentasContables",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ExigeTercero",
                table: "CuentasContables",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Naturaleza",
                table: "CuentasContables",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CuentasContables_CuentaPadreId",
                table: "CuentasContables",
                column: "CuentaPadreId");

            migrationBuilder.AddForeignKey(
                name: "FK_CuentasContables_CuentasContables_CuentaPadreId",
                table: "CuentasContables",
                column: "CuentaPadreId",
                principalTable: "CuentasContables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CuentasContables_CuentasContables_CuentaPadreId",
                table: "CuentasContables");

            migrationBuilder.DropIndex(
                name: "IX_CuentasContables_CuentaPadreId",
                table: "CuentasContables");

            migrationBuilder.DropColumn(
                name: "CuentaPadreId",
                table: "CuentasContables");

            migrationBuilder.DropColumn(
                name: "ExigeTercero",
                table: "CuentasContables");

            migrationBuilder.DropColumn(
                name: "Naturaleza",
                table: "CuentasContables");
        }
    }
}
