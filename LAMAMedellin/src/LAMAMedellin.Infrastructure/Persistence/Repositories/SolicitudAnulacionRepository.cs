using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

public sealed class SolicitudAnulacionRepository(LamaDbContext dbContext) : ISolicitudAnulacionRepository
{
    public Task<List<SolicitudAnulacion>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SolicitudesAnulacion.ToListAsync(cancellationToken);
    }

    public Task<SolicitudAnulacion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.SolicitudesAnulacion.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public Task<bool> ExistePendienteAsync(Guid comprobanteId, CancellationToken cancellationToken = default)
    {
        return dbContext.SolicitudesAnulacion.AnyAsync(
            s => s.ComprobanteId == comprobanteId && s.Estado == EstadoSolicitudAnulacion.Pendiente,
            cancellationToken);
    }

    public async Task AddAsync(SolicitudAnulacion solicitud, CancellationToken cancellationToken = default)
    {
        await dbContext.SolicitudesAnulacion.AddAsync(solicitud, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
