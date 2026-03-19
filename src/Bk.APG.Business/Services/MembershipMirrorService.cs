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
    private readonly ILogger<MembershipMirrorService> _logger;

    public MembershipMirrorService(
        IMembershipCandidateRepository membershipCandidateRepository,
        IMembershipRepository membershipRepository,
        ILogger<MembershipMirrorService> logger)
    {
        _membershipCandidateRepository = membershipCandidateRepository;
        _membershipRepository = membershipRepository;
        _logger = logger;
    }

    public async Task MirrorOrDeleteMembershipForGeneralElection(Membership membership, bool deleteCandidate, bool wasMetadataChanged)
    {
        _logger.LogInformation("Mirror membership {MembershipId} for general election", membership.Id);

        var membershipCandidate = await _membershipCandidateRepository.GetByMembershipIdForUpdate(membership.Id);

        if (membershipCandidate != null)
        {
            if (membershipCandidate.GeneralElectionCommittee?.IsValidated == true && membershipCandidate.GeneralElectionCommittee?.CandidateListStateId == CandidateListState.Validated)
            {
                _logger.LogInformation("Membership candidate list already validated, skip mirror entries");
                return;
            }

            if (deleteCandidate)
            {
                // if the end date has been shortened or the election type is an ending one, we can delete the membershipCandidate
                await _membershipCandidateRepository.Delete(membershipCandidate);
                _logger.LogInformation("Membership candidate {MembershipCandidateId} deleted because of end date change in present data", membershipCandidate.Id);
            }
            else
            {
                if (membershipCandidate.GeneralElectionCommittee?.CandidateListStateId == CandidateListState.ReadyForFederalCouncilProposalForwarded && !wasMetadataChanged)
                {
                    _logger.LogInformation("No metadata change during 'BRA ready forwarded' state, skip mirror entries");
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

                _logger.LogInformation("Updated membership candidate {MembershipCandidateId} with data from current membership", membershipCandidate.Id);
            }
        }
    }

    public async Task CreateNewMembershipFromCandidate(MembershipCreateDto createDto, string userName)
    {
        _logger.LogInformation("Create membership from GE for person {PersonId} in committee {CommitteeId}", createDto.PersonId, createDto.CommitteeId);

        var membership = MembershipMapper.FromMembershipCreateDto(createDto, userName);

        var newMembership = await _membershipRepository.Create(membership);

        _logger.LogInformation("Created membership from candidate with id {MembershipId}", newMembership.Id);
    }

    public async Task UpdateMembershipFromCandidate(Guid id, MembershipUpdateDto updateDto, string userName)
    {
        _logger.LogInformation("Update membership with id {MembershipId} started.", id);

        var existingEntry = await _membershipRepository.GetByIdForUpdate(id);

        existingEntry.BeginDate = updateDto.BeginDate;
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
        _logger.LogInformation("Updated candidate data to membership with with id {MembershipId}", id);
    }
}
