using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAMAMedellin.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Baseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bancos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumeroCuenta = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SaldoActual = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bancos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CentrosCosto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CentrosCosto", x => x.Id);
                });

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
                    Naturaleza = table.Column<int>(type: "int", nullable: false),
                    PermiteMovimiento = table.Column<bool>(type: "bit", nullable: false),
                    ExigeTercero = table.Column<bool>(type: "bit", nullable: false),
                    CuentaPadreId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentasContables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CuentasContables_CuentasContables_CuentaPadreId",
                        column: x => x.CuentaPadreId,
                        principalTable: "CuentasContables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CuotasAsamblea",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    ValorMensualCOP = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MesInicioCobro = table.Column<int>(type: "int", nullable: false),
                    ActaSoporte = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuotasAsamblea", x => x.Id);
                });

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
                name: "Eventos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FechaProgramada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LugarEncuentro = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Destino = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TipoEvento = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eventos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Miembros",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentoIdentidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombres = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Apodo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaIngreso = table.Column<DateOnly>(type: "date", nullable: false),
                    Rango = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    TipoSangre = table.Column<int>(type: "int", nullable: false),
                    NombreContactoEmergencia = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TelefonoContactoEmergencia = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MarcaMoto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModeloMoto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cilindraje = table.Column<int>(type: "int", nullable: false),
                    Placa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Miembros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TarifasCuota",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoAfiliacion = table.Column<int>(type: "int", nullable: false),
                    ValorMensualCOP = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TarifasCuota", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EntraObjectId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Rol = table.Column<int>(type: "int", nullable: false, defaultValue: 4),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MiembroId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProyectosSociales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CentroCostoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PresupuestoEstimado = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectosSociales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProyectosSociales_CentrosCosto_CentroCostoId",
                        column: x => x.CentroCostoId,
                        principalTable: "CentrosCosto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transacciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MontoCOP = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    MedioPago = table.Column<int>(type: "int", nullable: false),
                    CentroCostoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BancoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MonedaOrigen = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    MontoMonedaOrigen = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    TasaCambioUsada = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    FechaTasaCambio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FuenteTasaCambio = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transacciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transacciones_Bancos_BancoId",
                        column: x => x.BancoId,
                        principalTable: "Bancos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transacciones_CentrosCosto_CentroCostoId",
                        column: x => x.CentroCostoId,
                        principalTable: "CentrosCosto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.CreateTable(
                name: "Cajas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TipoCaja = table.Column<int>(type: "int", nullable: false),
                    SaldoActual = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CuentaContableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "ConceptosCobro",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ValorCOP = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PeriodicidadMensual = table.Column<int>(type: "int", nullable: false),
                    CuentaContableIngresoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConceptosCobro", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConceptosCobro_CuentasContables_CuentaContableIngresoId",
                        column: x => x.CuentaContableIngresoId,
                        principalTable: "CuentasContables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CodigoSKU = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PrecioVenta = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CantidadEnStock = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CantidadMinima = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CuentaContableIngresoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Producto_CuentaContable_Ingreso",
                        column: x => x.CuentaContableIngresoId,
                        principalTable: "CuentasContables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    FormaDonacion = table.Column<int>(type: "int", nullable: false),
                    MedioPagoODescripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
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

            migrationBuilder.CreateTable(
                name: "AsistenciasEvento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MiembroId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Asistio = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsistenciasEvento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AsistenciasEvento_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AsistenciasEvento_Miembros_MiembroId",
                        column: x => x.MiembroId,
                        principalTable: "Miembros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Beneficiarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProyectoSocialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NombreCompleto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TipoDocumento = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NumeroDocumento = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TieneConsentimientoHabeasData = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beneficiarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Beneficiarios_ProyectosSociales_ProyectoSocialId",
                        column: x => x.ProyectoSocialId,
                        principalTable: "ProyectosSociales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Egresos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Concepto = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TerceroId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CuentaContableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CajaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CentroCostoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComprobanteContableId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Egresos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Egresos_Cajas_CajaId",
                        column: x => x.CajaId,
                        principalTable: "Cajas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Egresos_CentrosCosto_CentroCostoId",
                        column: x => x.CentroCostoId,
                        principalTable: "CentrosCosto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Egresos_Comprobantes_ComprobanteContableId",
                        column: x => x.ComprobanteContableId,
                        principalTable: "Comprobantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Egresos_CuentasContables_CuentaContableId",
                        column: x => x.CuentaContableId,
                        principalTable: "CuentasContables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ingresos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Concepto = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TerceroId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CuentaContableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CajaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CentroCostoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComprobanteContableId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingresos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ingresos_Cajas_CajaId",
                        column: x => x.CajaId,
                        principalTable: "Cajas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ingresos_CentrosCosto_CentroCostoId",
                        column: x => x.CentroCostoId,
                        principalTable: "CentrosCosto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ingresos_Comprobantes_ComprobanteContableId",
                        column: x => x.ComprobanteContableId,
                        principalTable: "Comprobantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ingresos_CuentasContables_CuentaContableId",
                        column: x => x.CuentaContableId,
                        principalTable: "CuentasContables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CuentasPorCobrar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MiembroId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConceptoCobroId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000")),
                    FechaEmision = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "GETDATE()"),
                    FechaVencimiento = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "GETDATE()"),
                    ValorTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    SaldoPendiente = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentasPorCobrar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CuentasPorCobrar_ConceptosCobro_ConceptoCobroId",
                        column: x => x.ConceptoCobroId,
                        principalTable: "ConceptosCobro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuentasPorCobrar_Miembros_MiembroId",
                        column: x => x.MiembroId,
                        principalTable: "Miembros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosInventario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoMovimiento = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Concepto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosInventario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimientoInventario_Producto",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_AsistenciasEvento_EventoId_MiembroId",
                table: "AsistenciasEvento",
                columns: new[] { "EventoId", "MiembroId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AsistenciasEvento_MiembroId",
                table: "AsistenciasEvento",
                column: "MiembroId");

            migrationBuilder.CreateIndex(
                name: "IX_Bancos_NumeroCuenta",
                table: "Bancos",
                column: "NumeroCuenta",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiarios_ProyectoSocialId",
                table: "Beneficiarios",
                column: "ProyectoSocialId");

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiarios_TipoDocumento_NumeroDocumento",
                table: "Beneficiarios",
                columns: new[] { "TipoDocumento", "NumeroDocumento" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cajas_CuentaContableId",
                table: "Cajas",
                column: "CuentaContableId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comprobantes_NumeroConsecutivo",
                table: "Comprobantes",
                column: "NumeroConsecutivo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConceptosCobro_CuentaContableIngresoId",
                table: "ConceptosCobro",
                column: "CuentaContableIngresoId");

            migrationBuilder.CreateIndex(
                name: "IX_ConceptosCobro_Nombre",
                table: "ConceptosCobro",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CuentasContables_Codigo",
                table: "CuentasContables",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CuentasContables_CuentaPadreId",
                table: "CuentasContables",
                column: "CuentaPadreId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasPorCobrar_ConceptoCobroId",
                table: "CuentasPorCobrar",
                column: "ConceptoCobroId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasPorCobrar_MiembroConceptoFecha",
                table: "CuentasPorCobrar",
                columns: new[] { "MiembroId", "ConceptoCobroId", "FechaEmision" },
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Egresos_CajaId",
                table: "Egresos",
                column: "CajaId");

            migrationBuilder.CreateIndex(
                name: "IX_Egresos_CentroCostoId",
                table: "Egresos",
                column: "CentroCostoId");

            migrationBuilder.CreateIndex(
                name: "IX_Egresos_ComprobanteContableId",
                table: "Egresos",
                column: "ComprobanteContableId");

            migrationBuilder.CreateIndex(
                name: "IX_Egresos_CuentaContableId",
                table: "Egresos",
                column: "CuentaContableId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingresos_CajaId",
                table: "Ingresos",
                column: "CajaId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingresos_CentroCostoId",
                table: "Ingresos",
                column: "CentroCostoId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingresos_ComprobanteContableId",
                table: "Ingresos",
                column: "ComprobanteContableId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingresos_CuentaContableId",
                table: "Ingresos",
                column: "CuentaContableId");

            migrationBuilder.CreateIndex(
                name: "IX_Miembros_DocumentoIdentidad",
                table: "Miembros",
                column: "DocumentoIdentidad",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoInventario_Fecha",
                table: "MovimientosInventario",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_ProductoId",
                table: "MovimientosInventario",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_CodigoSKU_Unique",
                table: "Productos",
                column: "CodigoSKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Productos_CuentaContableIngresoId",
                table: "Productos",
                column: "CuentaContableIngresoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProyectosSociales_CentroCostoId",
                table: "ProyectosSociales",
                column: "CentroCostoId");

            migrationBuilder.CreateIndex(
                name: "IX_TarifasCuota_TipoAfiliacion",
                table: "TarifasCuota",
                column: "TipoAfiliacion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transacciones_BancoId",
                table: "Transacciones",
                column: "BancoId");

            migrationBuilder.CreateIndex(
                name: "IX_Transacciones_CentroCostoId",
                table: "Transacciones",
                column: "CentroCostoId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_EntraObjectId",
                table: "Usuarios",
                column: "EntraObjectId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsientosContables");

            migrationBuilder.DropTable(
                name: "AsistenciasEvento");

            migrationBuilder.DropTable(
                name: "Beneficiarios");

            migrationBuilder.DropTable(
                name: "CuentasPorCobrar");

            migrationBuilder.DropTable(
                name: "CuotasAsamblea");

            migrationBuilder.DropTable(
                name: "Donaciones");

            migrationBuilder.DropTable(
                name: "Egresos");

            migrationBuilder.DropTable(
                name: "Ingresos");

            migrationBuilder.DropTable(
                name: "MovimientosInventario");

            migrationBuilder.DropTable(
                name: "TarifasCuota");

            migrationBuilder.DropTable(
                name: "Transacciones");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Eventos");

            migrationBuilder.DropTable(
                name: "ProyectosSociales");

            migrationBuilder.DropTable(
                name: "ConceptosCobro");

            migrationBuilder.DropTable(
                name: "Miembros");

            migrationBuilder.DropTable(
                name: "Donantes");

            migrationBuilder.DropTable(
                name: "Cajas");

            migrationBuilder.DropTable(
                name: "Comprobantes");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Bancos");

            migrationBuilder.DropTable(
                name: "CentrosCosto");

            migrationBuilder.DropTable(
                name: "CuentasContables");
        }
    }
}
