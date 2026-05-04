using Bk.APG.Business.Dtos;
using Bk.APG.Business.Extensions;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;

namespace Bk.APG.Business.Services;

public class EiamAssignmentService : IEiamAssignmentService
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IWorklistTaskRepository _worklistTaskRepository;
    private readonly IGeneralElectionCommitteeRepository _generalElectionCommitteeRepository;

    public EiamAssignmentService(IAuthorizationService authorizationService, IWorklistTaskRepository worklistTaskRepository, IGeneralElectionCommitteeRepository generalElectionCommitteeRepository)
    {
        _authorizationService = authorizationService;
        _worklistTaskRepository = worklistTaskRepository;
        _generalElectionCommitteeRepository = generalElectionCommitteeRepository;
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

        var committee = await _generalElectionCommitteeRepository.GetByCommitteeId(committeeId);

        var isSecretariatTask = false;

        if (committee != null)
        {
            var generalElectionCommitteeTasks = (await _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(committee.Id)).ToList();
            var candidateListTasks = generalElectionCommitteeTasks
                .Where(x => x.WorklistTaskTypeId == WorklistTaskType.CandidateListCreate || x.WorklistTaskTypeId == WorklistTaskType.CandidateListForward || x.WorklistTaskTypeId == WorklistTaskType.CandidateListApprove).ToList();
            var activeCandidateListTask = candidateListTasks.FirstOrDefault(x => x.WorklistTaskStateId == WorklistTaskState.Active);

            // find out, if the current active task is assigne to secretariat. If yes, forwarding to secretarait is not possible in dialog! (BKDO-3493)
            if (activeCandidateListTask != null && activeCandidateListTask.AssignedTo!.Role == Role.Secretariat)
            {
                isSecretariatTask = true;
            }
        }

        var availableAssignments = currentEiamAssignment.GetAssignmentsForCandidateListForward(committeeId, isSecretariatTask);
        return availableAssignments.Select(x => EiamAssignmentMapper.ToDto(x));
    }

    public async Task<IEnumerable<EiamAssignmentDto>> GetAllForReadyForProposalForward(Guid committeeId)
    {
        var currentEiamAssignment = await _authorizationService.GetCurrentEiamAssignment();
        var tasks = await _worklistTaskRepository.GetAllByCommitteeId(committeeId);
        var availableAssignments = currentEiamAssignment.GetAssignmentsForReadyForProposalForward(committeeId, tasks).ToList();
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
            var eiamAssignment = await _authorizationService.GetCurrentEiamAssignment();
            departmentId = showDepartmentData && eiamAssignment.DepartmentId != null ? (Guid)eiamAssignment.DepartmentId : departmentId;
            officeId = showOfficeData && eiamAssignment.OfficeId != null ? (Guid)eiamAssignment.OfficeId : officeId;
            committeeId = showSecretariatData && eiamAssignment.CommitteeId != null ? (Guid)eiamAssignment.CommitteeId : committeeId;
        }

        return (departmentId, officeId, committeeId);
    }
}
