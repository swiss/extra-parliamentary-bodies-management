using Bk.APG.Business.Dtos;

namespace Bk.APG.Business.Services;

public interface IEiamAssignmentService
{
    Task<IEnumerable<EiamAssignmentDto>> GetAvailableAssignments();
    Task<IEnumerable<EiamAssignmentDto>> GetAllForCandidateListForward(Guid committeeId);
    Task<EiamAssignmentDto> GetCurrentEiamAssignment();
    Task<(Guid departmentId, Guid officeId, Guid committeeId)> GetPermittedIds();
}
