using System.Globalization;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Application.Features.Contabilidad.Commands.RegistrarComprobante;
using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Migracion.Commands.ImportarHistorico;

/// <summary>
/// Handler que procesa el archivo CSV histórico y genera comprobantes contables.
/// Implementa reglas de partida doble según el tipo de movimiento (Saldo, Ingreso, Egreso).
/// </summary>
public sealed class ImportarHistoricoCommandHandler
    : IRequestHandler<ImportarHistoricoCommand, ImportarHistoricoResult>
{
    private readonly ISender _sender;
    private readonly ICuentaContableRepository _cuentaRepository;
    private readonly IMiembroRepository _miembroRepository;
    private readonly ICentroCostoRepository _centroRepository;

    // Códigos PUC según Plan Único de Cuentas para Colombia
    private const string CodigoCajaBancos = "110505"; // Caja General
    private const string CodigoPatrimonioSaldos = "370505"; // Aportes Iniciales
    private const string CodigoIngresosCuotas = "421005"; // Cuotas ordinarias
    private const string CodigoIngresosVentas = "413005"; // Ventas de mercancía
    private const string CodigoGastosGenerales = "519595"; // Otros gastos
    private const string CodigoGastosTransporte = "513015"; // Transporte y fletes
    private const string CodigoGastosParches = "519520"; // Actividades deportivas

    public ImportarHistoricoCommandHandler(
        ISender sender,
        ICuentaContableRepository cuentaRepository,
        IMiembroRepository miembroRepository,
        ICentroCostoRepository centroRepository)
    {
        _sender = sender;
        _cuentaRepository = cuentaRepository;
        _miembroRepository = miembroRepository;
        _centroRepository = centroRepository;
    }

    public async Task<ImportarHistoricoResult> Handle(
        ImportarHistoricoCommand request,
        CancellationToken cancellationToken)
    {
        var advertencias = new List<string>();
        var lineasProcesadas = 0;
        var comprobantesCreados = 0;

        try
        {
            // 1. Obtener centro de costo por defecto (primer centro activo)
            var centros = await _centroRepository.GetAllAsync();
            var centroCostoId = centros.FirstOrDefault()?.Id
                ?? throw new InvalidOperationException("No hay centros de costo configurados");

            // 2. Obtener cuentas contables necesarias
            var cuentaCaja = await _cuentaRepository.GetByCodigoAsync(CodigoCajaBancos)
                ?? throw new InvalidOperationException($"Cuenta {CodigoCajaBancos} (Caja/Bancos) no existe");

            var cuentaPatrimonio = await _cuentaRepository.GetByCodigoAsync(CodigoPatrimonioSaldos)
                ?? throw new InvalidOperationException($"Cuenta {CodigoPatrimonioSaldos} (Patrimonio) no existe");

            var cuentaIngresosCuotas = await _cuentaRepository.GetByCodigoAsync(CodigoIngresosCuotas);
            var cuentaIngresosVentas = await _cuentaRepository.GetByCodigoAsync(CodigoIngresosVentas);
            var cuentaGastosGenerales = await _cuentaRepository.GetByCodigoAsync(CodigoGastosGenerales);
            var cuentaGastosTransporte = await _cuentaRepository.GetByCodigoAsync(CodigoGastosTransporte);
            var cuentaGastosParches = await _cuentaRepository.GetByCodigoAsync(CodigoGastosParches);

            // 3. Obtener miembros activos y construir diccionario de búsqueda
            var miembros = await _miembroRepository.GetActivosAsync();
            var diccionarioMiembros = ConstruirDiccionarioMiembros(miembros);

            // 4. Crear tercero genérico para movimientos sin miembro específico
            var terceroGenericoId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            // Asumimos que existe un miembro genérico con este ID

            // 5. Leer contenido CSV recibido en el comando
            if (string.IsNullOrWhiteSpace(request.ContenidoCsv))
            {
                throw new InvalidOperationException("El contenido CSV es obligatorio para ejecutar la migración.");
            }

            var lineasCsv = request.ContenidoCsv
                .Replace("\r\n", "\n", StringComparison.Ordinal)
                .Replace("\r", "\n", StringComparison.Ordinal)
                .Split('\n');

            // 6. Procesar cada línea del CSV (saltar encabezado)
            for (var index = 1; index < lineasCsv.Length; index++)
            {
                var lineaNumero = index + 1;
                var linea = lineasCsv[index];
                if (string.IsNullOrWhiteSpace(linea)) continue;

                try
                {
                    var campos = ParsearLineaCsv(linea);
                    if (campos.Length < 4)
                    {
                        advertencias.Add($"Línea {lineaNumero}: cantidad de columnas inválida.");
                        continue;
                    }

                    var fecha = ParsearFecha(campos[0]);
                    var concepto = campos[1].Trim();
                    var ingresos = ParsearMontoMoneda(campos[2]);
                    var egresos = ParsearMontoMoneda(campos[3]);

                    lineasProcesadas++;

                    // 7. Determinar tipo de movimiento y generar comprobante
                    if (concepto.Contains("SALDO EFECTIVO", StringComparison.OrdinalIgnoreCase))
                    {
                        // Saldo inicial: Débito Caja, Crédito Patrimonio
                        var monto = ingresos > 0 ? ingresos : egresos;
                        var comando = new RegistrarComprobanteCommand(
                            Fecha: fecha,
                            Tipo: TipoComprobante.Ingreso,
                            Descripcion: concepto,
                            Asientos: new List<RegistrarAsientoComprobanteDto>
                            {
                                new(cuentaCaja.Id, terceroGenericoId, centroCostoId,
                                    Debe: monto, Haber: 0, Referencia: "Migración CSV"),
                                new(cuentaPatrimonio.Id, terceroGenericoId, centroCostoId,
                                    Debe: 0, Haber: monto, Referencia: "Migración CSV")
                            }
                        );
                        await _sender.Send(comando, cancellationToken);
                        comprobantesCreados++;
                    }
                    else if (ingresos > 0)
                    {
                        // Ingreso: Débito Caja, Crédito Ingresos
                        var cuentaIngreso = DeterminarCuentaIngreso(
                            concepto, cuentaIngresosCuotas, cuentaIngresosVentas, cuentaGastosGenerales);

                        if (cuentaIngreso == null)
                        {
                            advertencias.Add($"Línea {lineasProcesadas}: cuenta de ingreso no configurada para '{concepto}'");
                            continue;
                        }

                        var terceroId = BuscarTerceroPorConcepto(concepto, diccionarioMiembros)
                            ?? terceroGenericoId;

                        var comando = new RegistrarComprobanteCommand(
                            Fecha: fecha,
                            Tipo: TipoComprobante.Ingreso,
                            Descripcion: concepto,
                            Asientos: new List<RegistrarAsientoComprobanteDto>
                            {
                                new(cuentaCaja.Id, terceroId, centroCostoId,
                                    Debe: ingresos, Haber: 0, Referencia: "Migración CSV"),
                                new(cuentaIngreso.Id, terceroId, centroCostoId,
                                    Debe: 0, Haber: ingresos, Referencia: "Migración CSV")
                            }
                        );
                        await _sender.Send(comando, cancellationToken);
                        comprobantesCreados++;
                    }
                    else if (egresos > 0)
                    {
                        // Egreso: Crédito Caja, Débito Gastos
                        var cuentaGasto = DeterminarCuentaGasto(
                            concepto, cuentaGastosParches, cuentaGastosTransporte, cuentaGastosGenerales);

                        if (cuentaGasto == null)
                        {
                            advertencias.Add($"Línea {lineasProcesadas}: cuenta de gasto no configurada para '{concepto}'");
                            continue;
                        }

                        var terceroId = BuscarTerceroPorConcepto(concepto, diccionarioMiembros)
                            ?? terceroGenericoId;

                        var comando = new RegistrarComprobanteCommand(
                            Fecha: fecha,
                            Tipo: TipoComprobante.Egreso,
                            Descripcion: concepto,
                            Asientos: new List<RegistrarAsientoComprobanteDto>
                            {
                                new(cuentaGasto.Id, terceroId, centroCostoId,
                                    Debe: egresos, Haber: 0, Referencia: "Migración CSV"),
                                new(cuentaCaja.Id, terceroId, centroCostoId,
                                    Debe: 0, Haber: egresos, Referencia: "Migración CSV")
                            }
                        );
                        await _sender.Send(comando, cancellationToken);
                        comprobantesCreados++;
                    }
                }
                catch (Exception ex)
                {
                    advertencias.Add($"Línea {lineaNumero}: {ex.Message}");
                }
            }

            return new ImportarHistoricoResult(comprobantesCreados, lineasProcesadas, advertencias);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static Dictionary<string, Guid> ConstruirDiccionarioMiembros(
        IEnumerable<Domain.Entities.Miembro> miembros)
    {
        var diccionario = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

        foreach (var miembro in miembros)
        {
            var nombreCompleto = $"{miembro.Nombre} {miembro.Apellidos}".Trim();
            var nombreInvertido = $"{miembro.Apellidos} {miembro.Nombre}".Trim();

            diccionario[nombreCompleto] = miembro.Id;
            diccionario[nombreInvertido] = miembro.Id;
            diccionario[miembro.Nombre] = miembro.Id;
            diccionario[miembro.Apellidos] = miembro.Id;
        }

        return diccionario;
    }

    private static Guid? BuscarTerceroPorConcepto(
        string concepto,
        Dictionary<string, Guid> diccionarioMiembros)
    {
        foreach (var kvp in diccionarioMiembros)
        {
            if (concepto.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
            {
                return kvp.Value;
            }
        }
        return null;
    }

    private static Domain.Entities.CuentaContable? DeterminarCuentaIngreso(
        string concepto,
        Domain.Entities.CuentaContable? cuentaCuotas,
        Domain.Entities.CuentaContable? cuentaVentas,
        Domain.Entities.CuentaContable? cuentaGeneral)
    {
        if (concepto.Contains("MENSUALIDAD", StringComparison.OrdinalIgnoreCase) ||
            concepto.Contains("CUOTA", StringComparison.OrdinalIgnoreCase))
            return cuentaCuotas;

        if (concepto.Contains("VENTA", StringComparison.OrdinalIgnoreCase) ||
            concepto.Contains("CAMISETA", StringComparison.OrdinalIgnoreCase) ||
            concepto.Contains("BUFF", StringComparison.OrdinalIgnoreCase))
            return cuentaVentas;

        return cuentaGeneral;
    }

    private static Domain.Entities.CuentaContable? DeterminarCuentaGasto(
        string concepto,
        Domain.Entities.CuentaContable? cuentaParches,
        Domain.Entities.CuentaContable? cuentaTransporte,
        Domain.Entities.CuentaContable? cuentaGeneral)
    {
        if (concepto.Contains("PARCHE", StringComparison.OrdinalIgnoreCase))
            return cuentaParches;

        if (concepto.Contains("TRANSPORTADORA", StringComparison.OrdinalIgnoreCase) ||
            concepto.Contains("TRANSPORTE", StringComparison.OrdinalIgnoreCase))
            return cuentaTransporte;

        return cuentaGeneral;
    }

    private static string[] ParsearLineaCsv(string linea)
    {
        // Parser simple para CSV con formato: FECHA,CONCEPTO,INGRESOS,EGRESOS
        // Maneja comillas dobles y comas dentro de campos
        var campos = new List<string>();
        var campoActual = new System.Text.StringBuilder();
        var dentroComillas = false;

        for (int i = 0; i < linea.Length; i++)
        {
            var c = linea[i];

            if (c == '"')
            {
                dentroComillas = !dentroComillas;
            }
            else if (c == ',' && !dentroComillas)
            {
                campos.Add(campoActual.ToString());
                campoActual.Clear();
            }
            else
            {
                campoActual.Append(c);
            }
        }
        campos.Add(campoActual.ToString()); // Último campo

        return campos.ToArray();
    }

    private static DateTime ParsearFecha(string fecha)
    {
        var valor = fecha.Trim().Trim('"');
        var formatos = new[] { "M/d/yyyy", "MM/dd/yyyy", "M/dd/yyyy", "MM/d/yyyy" };

        if (DateTime.TryParseExact(
                valor,
                formatos,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var resultado))
        {
            return resultado;
        }

        throw new FormatException($"String '{valor}' was not recognized as a valid DateTime.");
    }

    private static decimal ParsearMontoMoneda(string monto)
    {
        if (string.IsNullOrWhiteSpace(monto)) return 0;

        // Formato esperado: "$6,915,000", " $60,000 ", "-", ""
        var limpio = monto
            .Replace("$", "")
            .Replace(",", "")
            .Replace("\u00A0", "")
            .Replace("\t", "")
            .Replace("\r", "")
            .Replace("\n", "")
            .Replace("\"", "")
            .Replace(" ", "")
            .Trim();

        if (string.IsNullOrEmpty(limpio) || limpio == "-")
        {
            return 0;
        }

        if (decimal.TryParse(limpio, NumberStyles.Number, CultureInfo.InvariantCulture, out var valor))
        {
            return valor;
        }

        throw new FormatException($"The input string '{limpio}' was not in a correct format.");
    }
}
