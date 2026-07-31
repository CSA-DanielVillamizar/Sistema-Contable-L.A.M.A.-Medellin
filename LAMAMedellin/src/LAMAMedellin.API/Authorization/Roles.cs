namespace LAMAMedellin.API.Authorization;

/// <summary>
/// Agrupaciones de roles de la matriz de permisos del BRD (seccion 9).
///
/// Se centralizan aqui porque estaban repartidas como cadenas sueltas en cada
/// controlador, y 12 de los 19 no declaraban rol alguno: cualquier usuario
/// autenticado podia registrar comprobantes, crear miembros o administrar
/// usuarios. El control existia y funcionaba, pero solo estaba puesto en siete
/// sitios.
///
/// Convencion: cada modulo declara a nivel de clase quien puede LEER (la union
/// de la fila de la matriz) y sobreescribe en las acciones de escritura quien
/// puede ESCRIBIR. El atributo [Authorize(Roles = ...)] no distingue lectura de
/// escritura por si solo, asi que la distincion se hace por accion.
/// </summary>
public static class Roles
{
    public const string Admin = "Admin";
    public const string Tesorero = "Tesorero";
    public const string Contador = "Contador";
    public const string Operador = "Operador";
    public const string Junta = "Junta";
    public const string Logistica = "Logistica";
    public const string CapitanRuta = "CapitanRuta";
    public const string Inventario = "Inventario";

    /// <summary>Usuarios y roles internos: solo Admin, lectura y escritura.</summary>
    public const string SoloAdmin = Admin;

    /// <summary>Configuracion y catalogos. Admin escribe; el resto consulta.</summary>
    public const string ConfiguracionLectura = $"{Admin},{Operador},{Tesorero},{Contador}";
    public const string ConfiguracionEscritura = Admin;

    /// <summary>Tesoreria, CxC, CxP y donaciones comparten fila en la matriz.</summary>
    public const string TesoreriaLectura = $"{Admin},{Operador},{Tesorero},{Contador},{Junta}";
    public const string TesoreriaEscritura = $"{Operador},{Tesorero},{Admin}";

    /// <summary>Contabilidad: el contador es quien asienta; los demas consultan.</summary>
    public const string ContabilidadLectura = $"{Admin},{Operador},{Tesorero},{Contador},{Junta}";
    public const string ContabilidadEscritura = $"{Contador},{Admin}";

    /// <summary>Cierre mensual: Tesorero valida, Contador ejecuta, Junta ve.</summary>
    public const string CierreLectura = $"{Contador},{Tesorero},{Junta},{Admin}";
    public const string CierreValidar = $"{Tesorero},{Contador},{Admin}";
    public const string CierreEjecutar = $"{Contador},{Admin}";

    /// <summary>
    /// Beneficiarios: la Junta queda fuera a proposito. La matriz le concede
    /// solo datos agregados y el criterio de la historia 0-4 es explicito en
    /// que no puede ver PII de beneficiarios.
    /// </summary>
    public const string BeneficiariosLectura = $"{Admin},{Operador},{Tesorero},{Contador}";
    public const string BeneficiariosEscritura = $"{Operador},{Admin}";

    /// <summary>Proyectos: la Junta si puede consultarlos, sin bajar a PII.</summary>
    public const string ProyectosLectura = $"{Admin},{Operador},{Tesorero},{Contador},{Junta}";
    public const string ProyectosEscritura = $"{Operador},{Admin}";

    /// <summary>Negocios e inventario.</summary>
    public const string NegociosLectura = $"{Admin},{Operador},{Tesorero},{Contador},{Junta},{Inventario}";
    public const string NegociosEscritura = $"{Operador},{Admin},{Inventario}";

    /// <summary>Eventos y logistica del club.</summary>
    public const string EventosLectura = $"{Admin},{Operador},{Tesorero},{Contador},{Junta},{Logistica},{CapitanRuta}";
    public const string EventosEscritura = $"{Operador},{Admin},{Logistica},{CapitanRuta}";

    /// <summary>
    /// Miembros son datos maestros: los administra Admin y el resto los
    /// consulta, porque toda la cartera se emite contra ellos.
    /// </summary>
    public const string MiembrosLectura = $"{Admin},{Operador},{Tesorero},{Contador},{Junta}";
    public const string MiembrosEscritura = $"{Admin},{Operador}";

    /// <summary>Tableros y consultas agregadas: cualquier rol con sesion.</summary>
    public const string TodosLosRoles =
        $"{Admin},{Operador},{Tesorero},{Contador},{Junta},{Logistica},{CapitanRuta},{Inventario}";
}
