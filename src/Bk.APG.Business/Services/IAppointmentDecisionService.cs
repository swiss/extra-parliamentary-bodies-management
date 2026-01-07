using Bk.APG.Business.Dtos;

namespace Bk.APG.Business.Services;

public interface IAppointmentDecisionService
{
    Task<IEnumerable<AppointmentDecisionListDto>> GetAppointmentDecisionListByCommitteeId(Guid committeeId);
    Task<AppointmentDecisionListDto> CreateAppointmentDecision(AppointmentDecisionCreateDto createDto);
    AppointmentDecisionCreateDto GetEmpty();
    Task<AppointmentDecisionUpdateDto> GetByIdForUpdate(Guid appointmentDecisionId);
    Task<AppointmentDecisionListDto> UpdateAppointmentDecision(Guid id, AppointmentDecisionUpdateDto updateDto);
    Task DeleteAppointmentDecision(Guid id);
}
