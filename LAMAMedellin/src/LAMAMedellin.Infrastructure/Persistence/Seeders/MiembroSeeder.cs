using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Seeders;

public static class MiembroSeeder
{
    public static async Task SeedMiembrosAsync(this LamaDbContext context)
    {
        if (await context.Set<Miembro>().AnyAsync())
        {
            return;
        }

        // Miembro genérico con GUID fijo para operaciones sin tercero específico
        var miembroGenerico = new Miembro(
            "999999999",
            "Tercero",
            "Generico",
            "GEN",
            DateOnly.FromDateTime(DateTime.UtcNow),
            GrupoSanguineo.O_Positivo,
            "Contacto Generico",
            "3000000000",
            "Sin Marca",
            "Sin Modelo",
            150,
            "GEN000",
            null)
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001")
        };

        var miembros = new List<Miembro>
        {
            miembroGenerico,
            CrearMiembro("Hector Mario", "Gonzalez Henao", "8336963", "hecmarg@yahoo.com", "3104363831"),
            CrearMiembro("Ramon Antonio", "Gonzalez Castano", "15432593", "raangoca@gmail.com", "3137672573"),
            CrearMiembro("Jhon Harvey", "Gomez Patino", "9528949", "jhongo01@hotmail.com", "3006155416"),
            CrearMiembro("William Humberto", "Jimenez Perez", "98496540", "williamhjp@hotmail.com", "3017969572"),
            CrearMiembro("Carlos Alberto", "Araque Betancur", "71334468", "cocoloquisimo@gmail.com", "3206693638"),
            CrearMiembro("Milton Dario", "Gomez Rivera", "98589814", "miltondariog@gmail.com", "3183507127"),
            CrearMiembro("Carlos Mario", "Ceballos", "75049349", "carmace7@gmail.com", "3147244972"),
            CrearMiembro("Carlos Andres", "Perez Areiza", "98699136", "carlosap@gmail.com", "3017560517"),
            CrearMiembro("Juan Esteban", "Suarez Correa", "1095808546", "suarezcorreaj@gmail.com", "3156160015"),
            CrearMiembro("Jhon Emmanuel", "Arzuza Paez", "72345562", "jhonarzuza@gmail.com", "3003876340"),
            CrearMiembro("Jose Edinson", "Ospina Cruz", "8335981", "chattu.1964@hotmail.com", "3008542336"),
            CrearMiembro("Jefferson", "Montoya Munoz", "1128406344", "majayura2011@hotmail.com", "3508319246"),
            CrearMiembro("Anderson Arlex", "Betancur Rua", "1036634452", "armigas7@gmail.com", "3194207889"),
            CrearMiembro("Robinson Alejandro", "Galvis Parra", "71380596", "robin11952@hotmail.com", "3105127314"),
            CrearMiembro("Carlos Mario", "Diaz Diaz", "15506596", "carlosmario.diazdiaz@gmail.com", "3213167406"),
            CrearMiembro("Juan Esteban", "Osorio", "1128399797", "Juan.osorio1429@correo.policia.gov.co", "3112710782"),
            CrearMiembro("Carlos Julio", "Rendon Diaz", "8162536", "movie.cj@gmail.com", "3507757020"),
            CrearMiembro("Daniel Andrey", "Villamizar Araque", "8106002", "dvillamizara@gmail.com", "3106328171"),
            CrearMiembro("Jhon David", "Sanchez", "0", "jhonda361@gmail.com", "3013424220"),
            CrearMiembro("Angela Maria", "Rodriguez Ochoa", "43703788", "angelarodriguez40350@gmail.com", "3104490476"),
            CrearMiembro("Yeferson Bairon", "Usuga Agudelo", "0", "yeferson915@hotmail.com", "3002891509"),
            CrearMiembro("Jennifer Andrea", "Cardona Benitez", "1035424338", "tucoach21@gmail.com", "3014005382"),
            CrearMiembro("Laura Viviana", "Salazar Moreno", "1090419626", "laura.s.enf@hotmail.com", "3014307375"),
            CrearMiembro("Jose Julian", "Villamizar Araque", "8033065", "julianvilllamizar@outlook.com", "3014873771"),
            CrearMiembro("Gustavo Adolfo", "Gomez Zuluaga", "1094923731", string.Empty, "3132672208"),
            CrearMiembro("Nelson Augusto", "Montoya Mataute", "98472306", string.Empty, "3137100335"),
            CrearMiembro("Janeth Gisela", "Ospina Giraldo", "0", "chattu.1964@hotmail.com", "3008542336"),
            CrearMiembro("Maria Cristina", "Jimenez Angel", "0", "macrii2009@gmail.com", "3148103529"),
            CrearMiembro("Yolanda", "Echeverry Bohorquez", "0", "yolaeb@hotmail.com", "3113185201"),
            CrearMiembro("Linda Yulieth", "Zuleta Lopez", "0", "linzu28@hotmail.com", "3232611826"),
            CrearMiembro("Karol Natalia", "Santamaria Gonzalez", "0", "zeaelectricos@hotmail.com", "3146324621"),
            CrearMiembro("Lizet Natalia", "Gomez Franco", "0", "natygomez90@gmail.com", "3102874898"),
            CrearMiembro("Diana", "Larrota", "0", string.Empty, "3144009341"),
            CrearMiembro("Jazmin Adriana", "Rojas", "0", string.Empty, string.Empty),
            CrearMiembro("Leidy", "Hurtado", "0", string.Empty, "3209768510"),
            CrearMiembro("Natalia", "Londono Ocampo", "1094930482", string.Empty, string.Empty),
            CrearMiembro("Sandra", "Zapata", "21991558", string.Empty, "6042846744")
        };

        await context.Set<Miembro>().AddRangeAsync(miembros);
        await context.SaveChangesAsync();
    }

    private static Miembro CrearMiembro(
        string nombre,
        string apellidos,
        string documento,
        string correo,
        string celular)
    {
        var correoNormalizado = string.IsNullOrWhiteSpace(correo)
            ? $"sin-correo-{nombre.Replace(" ", "", StringComparison.OrdinalIgnoreCase).ToLowerInvariant()}.{apellidos.Replace(" ", "", StringComparison.OrdinalIgnoreCase).ToLowerInvariant()}@lama.local"
            : correo;

        var celularNormalizado = string.IsNullOrWhiteSpace(celular)
            ? "0000000000"
            : celular;

        var documentoNormalizado = NormalizarDocumento(documento, nombre, apellidos);

        return new Miembro(
            documentoNormalizado,
            nombre,
            apellidos,
            nombre.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? nombre,
            DateOnly.FromDateTime(DateTime.UtcNow),
            GrupoSanguineo.O_Positivo,
            $"Contacto {nombre}",
            celularNormalizado,
            "Harley-Davidson",
            "Softail",
            883,
            GenerarPlacaTemporal(documento),
            null);
    }

    /// <summary>
    /// Varios registros historicos llegaron sin numero de documento y quedaron
    /// cargados como "0". DocumentoIdentidad tiene indice unico, asi que repetir
    /// "0" hace fallar el seed completo. Se genera un marcador unico y evidente
    /// por persona en vez de duplicar el valor.
    /// </summary>
    private static string NormalizarDocumento(string documento, string nombre, string apellidos)
    {
        var limpio = (documento ?? string.Empty).Trim();

        if (limpio.Length > 0 && limpio != "0")
        {
            return limpio;
        }

        var iniciales = $"{nombre}{apellidos}"
            .Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .ToUpperInvariant();

        return $"SIN-DOC-{iniciales}";
    }

    private static string GenerarPlacaTemporal(string documento)
    {
        var soloDigitos = new string((documento ?? string.Empty).Where(char.IsDigit).ToArray());
        if (soloDigitos.Length < 3)
        {
            soloDigitos = soloDigitos.PadLeft(3, '0');
        }

        var sufijo = soloDigitos[^3..];
        return $"LAM{sufijo}";
    }
}
