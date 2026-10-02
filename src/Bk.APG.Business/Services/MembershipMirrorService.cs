using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Business.Services;

public class MembershipMirrorService : IMembershipMirrorService
{
    private readonly IMembershipCandidateRepository _membershipCandidateRepository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly IWorklistTaskRepository _worklistTaskRepository;
    private readonly IGeneralElectionCommitteeRepository _generalElectionCommitteeRepository;
    private readonly ILogger<MembershipMirrorService> _logger;

    public MembershipMirrorService(
        IMembershipCandidateRepository membershipCandidateRepository,
        IMembershipRepository membershipRepository,
        IAuthorizationService authorizationService,
        IWorklistTaskRepository worklistTaskRepository,
        IGeneralElectionCommitteeRepository generalElectionCommitteeRepository,
        ILogger<MembershipMirrorService> logger)
    {
        _membershipCandidateRepository = membershipCandidateRepository;
        _membershipRepository = membershipRepository;
        _authorizationService = authorizationService;
        _worklistTaskRepository = worklistTaskRepository;
        _generalElectionCommitteeRepository = generalElectionCommitteeRepository;
        _logger = logger;
    }

    public async Task MirrorOrDeleteMembershipForGeneralElection(Membership membership, bool deleteCandidate, bool wasMetadataChanged)
    {
        ArgumentNullException.ThrowIfNull(membership);

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Mirror membership {MembershipId} for general election", membership.Id);
        }

        var membershipCandidate = await _membershipCandidateRepository.GetByMembershipIdForUpdate(membership.Id);

        if (membershipCandidate != null)
        {
            if (membershipCandidate.GeneralElectionCommittee?.IsValidated == true && membershipCandidate.GeneralElectionCommittee?.CandidateListStateId == CandidateListState.Validated)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Membership candidate list already validated, skip mirror entries");
                }
                return;
            }

            if (deleteCandidate)
            {
                // if the end date has been shortened or the election type is an ending one, we can delete the membershipCandidate
                await _membershipCandidateRepository.Delete(membershipCandidate);
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Membership candidate {MembershipCandidateId} deleted because of end date change in present data", membershipCandidate.Id);
                }
            }
            else
            {
                if (membershipCandidate.GeneralElectionCommittee?.CandidateListStateId == CandidateListState.ReadyForFederalCouncilProposalForwarded && !wasMetadataChanged)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation("No metadata change during 'BRA ready forwarded' state, skip mirror entries");
                    }
                    return;
                }

                var updatedMembership = GeneralElectionMapper.ToMembershipCandidateMirrorDto(membership);

                membershipCandidate.MaximumEmploymentLevel = updatedMembership.MaximumEmploymentLevel;
                membershipCandidate.ElectionTypeId = updatedMembership.ElectionTypeId;
                membershipCandidate.FunctionId = updatedMembership.FunctionId;
                membershipCandidate.ElectionOfficeId = updatedMembership.ElectionOfficeId;
                membershipCandidate.MembershipAdditionId = updatedMembership.MembershipAdditionId;
                membershipCandidate.Remarks = updatedMembership.Remarks;
                membershipCandidate.RemarksStatus = updatedMembership.RemarksStatus;
                membershipCandidate.InCorrelationWithFederalDuty = updatedMembership.InCorrelationWithFederalDuty;
                membershipCandidate.Modified = updatedMembership.Modified;
                membershipCandidate.ModifiedBy = updatedMembership.ModifiedBy;
                membershipCandidate.InCorrelationWithFederalDuty = membership.InCorrelationWithFederalDuty;
                if (membershipCandidate.GeneralElectionCommittee?.CandidateListStateId != CandidateListState.ReadyForFederalCouncilProposalForwarded &&
                    membershipCandidate.GeneralElectionCommittee?.CandidateListStateId != CandidateListState.ReadyForFederalCouncilProposalFinalized)
                {
                    membershipCandidate.JustificationLongerDuty = membership.JustificationLongerDuty;
                    membershipCandidate.JustificationShorterDuty = membership.JustificationShorterDuty;
                    membershipCandidate.JustificationMemberInFederalAssembly = membership.JustificationMemberInFederalAssembly;
                    membershipCandidate.JustificationMemberInFederalDuty = membership.JustificationMemberInFederalDuty;
                    membershipCandidate.RequirementsProfile = membership.RequirementsProfile;
                }

                await _membershipCandidateRepository.CommitChanges();

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Updated membership candidate {MembershipCandidateId} with data from current membership", membershipCandidate.Id);
                }
            }
        }
    }

    public async Task CreateNewMembershipFromCandidate(MembershipCreateDto createDto, string userName, Guid? committeeTypeId = null)
    {
        ArgumentNullException.ThrowIfNull(createDto);

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Create membership from GE for person {PersonId} in committee {CommitteeId}", createDto.PersonId, createDto.CommitteeId);
        }

        var membership = MembershipMapper.FromMembershipCreateDto(createDto, userName, committeeTypeId);

        var newMembership = await _membershipRepository.Create(membership);

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Created membership from candidate with id {MembershipId}", newMembership.Id);
        }
    }

    public async Task UpdateMembershipFromCandidate(Guid id, MembershipUpdateDto updateDto, string userName)
    {
        ArgumentNullException.ThrowIfNull(updateDto);

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Update membership with id {MembershipId} started.", id);
        }

        var existingEntry = await _membershipRepository.GetByIdForUpdate(id);

        // As the candidates are updated, we should NOT reset the BeginDate here, as we overwrite the original value!
        existingEntry.PersonId = updateDto.PersonId;
        existingEntry.MaximumEmploymentLevel = updateDto.MaximumEmploymentLevel;
        existingEntry.EndDate = updateDto.EndDate;
        existingEntry.ElectionTypeId = updateDto.ElectionTypeId;
        existingEntry.FunctionId = updateDto.FunctionId;
        existingEntry.ElectionOfficeId = updateDto.ElectionOfficeId;
        existingEntry.MembershipAdditionId = updateDto.MembershipAdditionId;
        existingEntry.JustificationLongerDuty = updateDto.JustificationLongerDuty;
        existingEntry.JustificationShorterDuty = updateDto.JustificationShorterDuty;
        existingEntry.JustificationMemberInFederalDuty = updateDto.JustificationMemberInFederalDuty;
        existingEntry.JustificationMemberInFederalAssembly = updateDto.JustificationMemberInFederalAssembly;
        existingEntry.RequirementsProfile = updateDto.RequirementsProfile;
        existingEntry.Remarks = updateDto.Remarks;
        existingEntry.RemarksStatus = updateDto.RemarksStatus;
        existingEntry.InCorrelationWithFederalDuty = updateDto.InCorrelationWithFederalDuty;
        existingEntry.ModifiedBy = userName;
        existingEntry.Modified = DateTime.UtcNow;

        await _membershipRepository.CommitChanges();
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Updated candidate data to membership with with id {MembershipId}", id);
        }
    }

    public async Task InvalidateMembershipCandidateList(Guid committeeId)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Invalidating membership candidate list for  committee {CommitteeId}", committeeId);
        }

        var generalElectionCommittee = await _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId);
        generalElectionCommittee.IsValidated = false;
        generalElectionCommittee.CandidateListStateId = CandidateListState.Draft;
        var committeeTasks = await _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id);

        var taskApproveByDepartment = committeeTasks.FirstOrDefault(y => y.AssignedTo?.Role == Role.Department
            && y.WorklistTaskTypeId == WorklistTaskType.CandidateListApprove);
        var taskListForSecretariat = committeeTasks.Where(y => y.AssignedTo?.Role == Role.Secretariat
            && (y.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMissingJustifications ||
            y.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMissingSecretariat ||
            y.WorklistTaskTypeId == WorklistTaskType.GeneralElectionPersonBaseData ||
            y.WorklistTaskTypeId == WorklistTaskType.GeneralElectionPersonInterests ||
            y.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMissingDataProtectionOfficer ||
            y.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMembershipValidation));

        if (taskApproveByDepartment is not null)
        {
            taskApproveByDepartment.WorklistTaskStateId = WorklistTaskState.Active;
            taskApproveByDepartment.Modified = DateTime.UtcNow;
            taskApproveByDepartment.ModifiedBy = _authorizationService.GetCurrentUserName();
        }

        foreach (var task in taskListForSecretariat)
        {
            task.WorklistTaskStateId = WorklistTaskState.Inactive;
            task.Modified = DateTime.UtcNow;
            task.ModifiedBy = _authorizationService.GetCurrentUserName();
        }

        // Invalidate BRA Ready tasks

        var taskListForProposalAdminOrDepartmentOrOffice = committeeTasks.Where(y =>
           (y.AssignedTo?.Role == Role.Admin || y.AssignedTo?.Role == Role.Department || y.AssignedTo?.Role == Role.Office || y.AssignedTo?.Role == Role.Secretariat)
           && y.WorklistTaskTypeId == WorklistTaskType.ReadyForFederalCouncilProposal);

        foreach (var task in taskListForProposalAdminOrDepartmentOrOffice)
        {
            task.WorklistTaskStateId = WorklistTaskState.Inactive;
            task.Modified = DateTime.UtcNow;
            task.ModifiedBy = _authorizationService.GetCurrentUserName();
        }
        await _worklistTaskRepository.CommitChanges();
    }
}
