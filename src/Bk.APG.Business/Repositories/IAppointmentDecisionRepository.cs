using Bk.APG.Business.Models;

namespace Bk.APG.Business.Repositories;

public interface IAppointmentDecisionRepository
{
    Task<IEnumerable<AppointmentDecision>> GetAppointmentDecisionsByCommitteeId(Guid committeeId);
    void CreateForMigration(AppointmentDecision appointmentDecision);
    Task Create(AppointmentDecision appointmentDecision);
    Task<AppointmentDecision> GetAppointmentDecisionById(Guid appointmentDecisionId);
    Task<AppointmentDecision> GetAppointmentDecisionByIdForUpdate(Guid appointmentDecisionId);
    void Delete(AppointmentDecision appointmentDecision);
    Task CommitChanges();
}
