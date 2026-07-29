using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAMAMedellin.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConsolidarTesoreriaEnBanco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Egresos_Cajas_CajaId",
                table: "Egresos");

            migrationBuilder.DropForeignKey(
                name: "FK_Ingresos_Cajas_CajaId",
                table: "Ingresos");

            // ----------------------------------------------------------------
            // NOTA: el orden de esta migracion se ajusto a mano.
            //
            // El andamiaje generado borraba la tabla Cajas antes de crear la
            // clave foranea de Ingresos y Egresos hacia Bancos. Sobre una base
            // con datos eso falla: los movimientos existentes referencian Ids
            // de Caja que no existirian en Bancos.
            //
            // La solucion es convertir cada Caja en un Banco CONSERVANDO SU Id.
            // Asi los valores ya guardados en Ingresos.CajaId y Egresos.CajaId
            // siguen siendo validos tras el renombre a BancoId, y no hace falta
            // remapear una sola fila: la historia queda intacta.
            // ----------------------------------------------------------------

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Bancos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CuentaContableId",
                table: "Bancos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EsActivo",
                table: "Bancos",
                type: "bit",
                nullable: false,
                defaultValue: true);

            // Las cuentas bancarias que ya existian toman la cuenta contable de
            // bancos del PUC (111005) y quedan con un nombre legible.
            migrationBuilder.Sql(@"
                UPDATE b
                SET b.CuentaContableId = (SELECT TOP 1 Id FROM CuentasContables WHERE Codigo = '111005'),
                    b.Nombre = COALESCE(NULLIF(LTRIM(RTRIM(b.Nombre)), ''), 'Cuenta ' + b.NumeroCuenta)
                FROM Bancos b
                WHERE b.CuentaContableId IS NULL;
            ");

            // Cada Caja se convierte en Banco con el MISMO Id.
            //
            // Solo queda activa la que representa una cuenta bancaria real
            // (cuentas del grupo 1110 del PUC). Las de efectivo pasan a
            // inactivas: conservan su historia y su saldo, pero ya no admiten
            // movimientos nuevos, que es justamente la regla de operar 100%
            // bancarizado.
            migrationBuilder.Sql(@"
                INSERT INTO Bancos (Id, Nombre, NumeroCuenta, SaldoActual, CuentaContableId, EsActivo, IsDeleted, CreatedAt)
                SELECT
                    c.Id,
                    c.Nombre,
                    LEFT('MIGRADO-' + CAST(c.Id AS VARCHAR(36)), 50),
                    c.SaldoActual,
                    c.CuentaContableId,
                    CASE WHEN cc.Codigo LIKE '1110%' THEN 1 ELSE 0 END,
                    c.IsDeleted,
                    SYSUTCDATETIME()
                FROM Cajas c
                JOIN CuentasContables cc ON cc.Id = c.CuentaContableId
                WHERE NOT EXISTS (SELECT 1 FROM Bancos b WHERE b.Id = c.Id);
            ");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Bancos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "CuentaContableId",
                table: "Bancos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.DropTable(
                name: "Cajas");

            migrationBuilder.RenameColumn(
                name: "CajaId",
                table: "Ingresos",
                newName: "BancoId");

            migrationBuilder.RenameIndex(
                name: "IX_Ingresos_CajaId",
                table: "Ingresos",
                newName: "IX_Ingresos_BancoId");

            migrationBuilder.RenameColumn(
                name: "CajaId",
                table: "Egresos",
                newName: "BancoId");

            migrationBuilder.RenameIndex(
                name: "IX_Egresos_CajaId",
                table: "Egresos",
                newName: "IX_Egresos_BancoId");

            migrationBuilder.CreateIndex(
                name: "IX_Bancos_CuentaContableId",
                table: "Bancos",
                column: "CuentaContableId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bancos_CuentasContables_CuentaContableId",
                table: "Bancos",
                column: "CuentaContableId",
                principalTable: "CuentasContables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Egresos_Bancos_BancoId",
                table: "Egresos",
                column: "BancoId",
                principalTable: "Bancos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ingresos_Bancos_BancoId",
                table: "Ingresos",
                column: "BancoId",
                principalTable: "Bancos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bancos_CuentasContables_CuentaContableId",
                table: "Bancos");

            migrationBuilder.DropForeignKey(
                name: "FK_Egresos_Bancos_BancoId",
                table: "Egresos");

            migrationBuilder.DropForeignKey(
                name: "FK_Ingresos_Bancos_BancoId",
                table: "Ingresos");

            migrationBuilder.DropIndex(
                name: "IX_Bancos_CuentaContableId",
                table: "Bancos");

            migrationBuilder.DropColumn(
                name: "CuentaContableId",
                table: "Bancos");

            migrationBuilder.DropColumn(
                name: "EsActivo",
                table: "Bancos");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Bancos");

            migrationBuilder.RenameColumn(
                name: "BancoId",
                table: "Ingresos",
                newName: "CajaId");

            migrationBuilder.RenameIndex(
                name: "IX_Ingresos_BancoId",
                table: "Ingresos",
                newName: "IX_Ingresos_CajaId");

            migrationBuilder.RenameColumn(
                name: "BancoId",
                table: "Egresos",
                newName: "CajaId");

            migrationBuilder.RenameIndex(
                name: "IX_Egresos_BancoId",
                table: "Egresos",
                newName: "IX_Egresos_CajaId");

            migrationBuilder.CreateTable(
                name: "Cajas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CuentaContableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SaldoActual = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TipoCaja = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cajas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cajas_CuentasContables_CuentaContableId",
                        column: x => x.CuentaContableId,
                        principalTable: "CuentasContables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cajas_CuentaContableId",
                table: "Cajas",
                column: "CuentaContableId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Egresos_Cajas_CajaId",
                table: "Egresos",
                column: "CajaId",
                principalTable: "Cajas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ingresos_Cajas_CajaId",
                table: "Ingresos",
                column: "CajaId",
                principalTable: "Cajas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
