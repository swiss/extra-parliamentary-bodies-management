using Bk.APG.Business.Dtos;
using Bk.APG.Business.Extensions;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Repositories;

namespace Bk.APG.Business.Services;

public class EiamAssignmentService : IEiamAssignmentService
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IEiamAssignmentRepository _eiamAssignmentRepository;

    public EiamAssignmentService(IAuthorizationService authorizationService, IEiamAssignmentRepository eiamAssignmentRepository)
    {
        _authorizationService = authorizationService;
        _eiamAssignmentRepository = eiamAssignmentRepository;
    }

    public async Task<IEnumerable<EiamAssignmentDto>> GetAvailableAssignments()
    {
        var currentEiamAssignment = await _authorizationService.GetCurrentEiamAssignment();
        var availableAssignments = currentEiamAssignment.GetAssignableIds();
        return availableAssignments.Select(x => EiamAssignmentMapper.ToDto(x, true));
    }

    public async Task<IEnumerable<EiamAssignmentDto>> GetAllForCandidateListForward(Guid committeeId)
    {
        var currentEiamAssignment = await _authorizationService.GetCurrentEiamAssignment();
        var availableAssignments = currentEiamAssignment.GetAssignmentsForCandidateListForward(committeeId);
        return availableAssignments.Select(x => EiamAssignmentMapper.ToDto(x));
    }

    public async Task<IEnumerable<EiamAssignmentDto>> GetAllForReadyForProposalForward(Guid committeeId)
    {
        var currentEiamAssignment = await _authorizationService.GetCurrentEiamAssignment();
        var availableAssignments = currentEiamAssignment.GetAssignmentsForReadyForProposalForward(committeeId).ToList();
        return availableAssignments.Select(x => EiamAssignmentMapper.ToDto(x));
    }

    public async Task<EiamAssignmentDto> GetCurrentEiamAssignment()
    {
        var currentEiamAssignment = await _authorizationService.GetCurrentEiamAssignment();
        return EiamAssignmentMapper.ToDto(currentEiamAssignment);
    }

    public async Task<(Guid departmentId, Guid officeId, Guid committeeId)> GetPermittedIds()
    {
        var showAllData = _authorizationService.IsAdmin || _authorizationService.IsObserver;
        var showDepartmentData = _authorizationService.IsDepartment;
        var showOfficeData = _authorizationService.IsOffice;
        var showSecretariatData = _authorizationService.IsSecretariat;

        var departmentId = Guid.Empty;
        var officeId = Guid.Empty;
        var committeeId = Guid.Empty;

        if (!showAllData)
        {
            var eiamAssignment = await _eiamAssignmentRepository.GetByExternalId(_authorizationService.GetCurrentExternalId());
            departmentId = showDepartmentData && eiamAssignment.DepartmentId != null ? (Guid)eiamAssignment.DepartmentId : departmentId;
            officeId = showOfficeData && eiamAssignment.OfficeId != null ? (Guid)eiamAssignment.OfficeId : officeId;
            committeeId = showSecretariatData && eiamAssignment.CommitteeId != null ? (Guid)eiamAssignment.CommitteeId : committeeId;
        }

        return (departmentId, officeId, committeeId);
    }
}
