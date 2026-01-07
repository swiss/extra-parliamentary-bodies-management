using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.CrossCutting.Exception;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Infrastructure.DataSource.Repositories;

public class AppointmentDecisionRepository : IAppointmentDecisionRepository
{
    private readonly DataContext _dataContext;

    public AppointmentDecisionRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<IEnumerable<AppointmentDecision>> GetAppointmentDecisionsByCommitteeId(Guid committeeId)
    {
        return await GetAppointmentDecisions()
            .Include(x => x.OriginalDocument)
            .Where(x => x.CommitteeId == committeeId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<AppointmentDecision> GetAppointmentDecisionById(Guid appointmentDecisionId)
    {
        var appointmentDecision = await GetAppointmentDecisions().AsNoTracking().FirstOrDefaultAsync(x => x.Id == appointmentDecisionId);

        if (appointmentDecision is null)
        {
            throw new EntityNotFoundException($"Appointment Decision Id={appointmentDecisionId} not found");
        }

        return appointmentDecision;
    }

    public async Task<AppointmentDecision> GetAppointmentDecisionByIdForUpdate(Guid appointmentDecisionId)
    {
        var appointmentDecision = await GetAppointmentDecisions().FirstOrDefaultAsync(x => x.Id == appointmentDecisionId);

        if (appointmentDecision is null)
        {
            throw new EntityNotFoundException($"Appointment Decision Id={appointmentDecisionId} not found");
        }

        return appointmentDecision;
    }

    public void Delete(AppointmentDecision appointmentDecision)
    {
        _dataContext.AppointmentDecisions.Remove(appointmentDecision);
    }

    public void CreateForMigration(AppointmentDecision appointmentDecision)
    {
        _dataContext.AppointmentDecisions.Add(appointmentDecision);
    }

    private IQueryable<AppointmentDecision> GetAppointmentDecisions()
    {
        var values = _dataContext.AppointmentDecisions
            .Include(x => x.AppointmentDecisionLinkType)
            .Include(x => x.AppointmentDecisionType)
            .Include(x => x.FileReferenceGerman)
            .Include(x => x.FileReferenceFrench)
            .Include(x => x.FileReferenceItalian)
            .Include(x => x.FileReferenceRomansh);

        return values;
    }

    public async Task Create(AppointmentDecision appointmentDecision)
    {
        await _dataContext.AppointmentDecisions.AddAsync(appointmentDecision);
        await _dataContext.SaveChangesAsync();
    }

    public async Task CommitChanges()
    {
        await _dataContext.SaveChangesAsync();
    }
}
