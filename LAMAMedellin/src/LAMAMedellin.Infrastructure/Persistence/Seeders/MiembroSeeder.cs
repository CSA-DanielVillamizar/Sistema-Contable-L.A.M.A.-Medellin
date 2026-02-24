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

        var miembros = new List<Miembro>
        {
            CrearMiembro("Héctor Mario", "González Henao", "8336963", "hecmarg@yahoo.com", "3104363831", TipoAfiliacion.FullColor),
            CrearMiembro("Ramón Antonio", "González Castaño", "15432593", "raangoca@gmail.com", "3137672573", TipoAfiliacion.FullColor),
            CrearMiembro("Jhon Harvey", "Gómez Patiño", "9528949", "jhongo01@hotmail.com", "3006155416", TipoAfiliacion.FullColor),
            CrearMiembro("William Humberto", "Jiménez Perez", "98496540", "williamhjp@hotmail.com", "3017969572", TipoAfiliacion.FullColor),
            CrearMiembro("Carlos Alberto", "Araque Betancur", "71334468", "cocoloquisimo@gmail.com", "3206693638", TipoAfiliacion.FullColor),
            CrearMiembro("Milton Darío", "Gómez Rivera", "98589814", "miltondariog@gmail.com", "3183507127", TipoAfiliacion.FullColor),
            CrearMiembro("Carlos Mario", "Ceballos", "75049349", "carmace7@gmail.com", "3147244972", TipoAfiliacion.FullColor),
            CrearMiembro("Carlos Andrés", "Pérez Areiza", "98699136", "carlosap@gmail.com", "3017560517", TipoAfiliacion.FullColor),
            CrearMiembro("Juan Esteban", "Suárez Correa", "1095808546", "suarezcorreaj@gmail.com", "3156160015", TipoAfiliacion.FullColor),
            CrearMiembro("Jhon Emmanuel", "Arzuza Páez", "72345562", "jhonarzuza@gmail.com", "3003876340", TipoAfiliacion.FullColor),
            CrearMiembro("José Edinson", "Ospina Cruz", "8335981", "chattu.1964@hotmail.com", "3008542336", TipoAfiliacion.FullColor),
            CrearMiembro("Jefferson", "Montoya Muñoz", "1128406344", "majayura2011@hotmail.com", "3508319246", TipoAfiliacion.FullColor),
            CrearMiembro("Anderson Arlex", "Betancur Rua", "1036634452", "armigas7@gmail.com", "3194207889", TipoAfiliacion.FullColor),
            CrearMiembro("Robinson Alejandro", "Galvis Parra", "71380596", "robin11952@hotmail.com", "3105127314", TipoAfiliacion.FullColor),
            CrearMiembro("Carlos Mario", "Díaz Díaz", "15506596", "carlosmario.diazdiaz@gmail.com", "3213167406", TipoAfiliacion.FullColor),
            CrearMiembro("Juan Esteban", "Osorio", "1128399797", "Juan.osorio1429@correo.policia.gov.co", "3112710782", TipoAfiliacion.FullColor),
            CrearMiembro("Carlos Julio", "Rendón Díaz", "8162536", "movie.cj@gmail.com", "3507757020", TipoAfiliacion.FullColor),
            CrearMiembro("Daniel Andrey", "Villamizar Araque", "8106002", "dvillamizara@gmail.com", "3106328171", TipoAfiliacion.FullColor),
            CrearMiembro("Jhon David", "Sánchez", "0", "jhonda361@gmail.com", "3013424220", TipoAfiliacion.FullColor),
            CrearMiembro("Ángela Maria", "Rodríguez Ochoa", "43703788", "angelarodriguez40350@gmail.com", "3104490476", TipoAfiliacion.FullColor),
            CrearMiembro("Yeferson Bairon", "Úsuga Agudelo", "0", "yeferson915@hotmail.com", "3002891509", TipoAfiliacion.FullColor),
            CrearMiembro("Jennifer Andrea", "Cardona Benítez", "1035424338", "tucoach21@gmail.com", "3014005382", TipoAfiliacion.Rockets),
            CrearMiembro("Laura Viviana", "Salazar Moreno", "1090419626", "laura.s.enf@hotmail.com", "3014307375", TipoAfiliacion.FullColor),
            CrearMiembro("José Julián", "Villamizar Araque", "8033065", "julianvilllamizar@outlook.com", "3014873771", TipoAfiliacion.FullColor),
            CrearMiembro("Gustavo Adolfo", "Gómez Zuluaga", "1094923731", string.Empty, "3132672208", TipoAfiliacion.Rockets),
            CrearMiembro("Nelson Augusto", "Montoya Mataute", "98472306", string.Empty, "3137100335", TipoAfiliacion.Rockets),
            CrearMiembro("Janeth Gisela", "Ospina Giraldo", "0", "chattu.1964@hotmail.com", "3008542336", TipoAfiliacion.Esposa),
            CrearMiembro("María Cristina", "Jiménez Ángel", "0", "macrii2009@gmail.com", "3148103529", TipoAfiliacion.Esposa),
            CrearMiembro("Yolanda", "Echeverry Bohórquez", "0", "yolaeb@hotmail.com", "3113185201", TipoAfiliacion.Esposa),
            CrearMiembro("Linda Yulieth", "Zuleta López", "0", "linzu28@hotmail.com", "3232611826", TipoAfiliacion.Esposa),
            CrearMiembro("Karol Natalia", "Santamaria Gonzalez", "0", "zeaelectricos@hotmail.com", "3146324621", TipoAfiliacion.Esposa),
            CrearMiembro("Lizet Natalia", "Gómez Franco", "0", "natygomez90@gmail.com", "3102874898", TipoAfiliacion.Esposa),
            CrearMiembro("Diana", "Larrota", "0", string.Empty, "3144009341", TipoAfiliacion.Esposa),
            CrearMiembro("Jazmin Adriana", "Rojas", "0", string.Empty, string.Empty, TipoAfiliacion.Esposa),
            CrearMiembro("Leidy", "Hurtado", "0", string.Empty, "3209768510", TipoAfiliacion.Esposa),
            CrearMiembro("Natalia", "Londoño Ocampo", "1094930482", string.Empty, string.Empty, TipoAfiliacion.Esposa),
            CrearMiembro("Sandra", "Zapata", "21991558", string.Empty, "6042846744", TipoAfiliacion.Esposa)
        };

        await context.Set<Miembro>().AddRangeAsync(miembros);
        await context.SaveChangesAsync();
    }

    private static Miembro CrearMiembro(
        string nombre,
        string apellidos,
        string documento,
        string correo,
        string celular,
        TipoAfiliacion tipoAfiliacion)
    {
        var correoNormalizado = string.IsNullOrWhiteSpace(correo)
            ? $"sin-correo-{nombre.Replace(" ", "", StringComparison.OrdinalIgnoreCase).ToLowerInvariant()}.{apellidos.Replace(" ", "", StringComparison.OrdinalIgnoreCase).ToLowerInvariant()}@lama.local"
            : correo;

        var celularNormalizado = string.IsNullOrWhiteSpace(celular)
            ? "0000000000"
            : celular;

        return new Miembro(
            nombre,
            apellidos,
            documento,
            correoNormalizado,
            celularNormalizado,
            tipoAfiliacion,
            EstadoMiembro.Activo);
    }
}
