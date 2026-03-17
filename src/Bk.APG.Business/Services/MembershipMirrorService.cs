using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Business.Services;

public class MembershipMirrorService : IMembershipMirrorService
{
    private readonly IMembershipCandidateRepository _membershipCandidateRepository;
    private readonly ILogger<MembershipMirrorService> _logger;

    public MembershipMirrorService(
        IMembershipCandidateRepository membershipCandidateRepository,
        ILogger<MembershipMirrorService> logger)
    {
        _membershipCandidateRepository = membershipCandidateRepository;
        _logger = logger;
    }

    public async Task MirrorOrDeleteMembershipForGeneralElection(Membership membership, bool deleteCandidate)
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
                membershipCandidate.JustificationLongerDuty = membership.JustificationLongerDuty;
                membershipCandidate.JustificationShorterDuty = membership.JustificationShorterDuty;
                membershipCandidate.JustificationMemberInFederalAssembly = membership.JustificationMemberInFederalAssembly;
                membershipCandidate.JustificationMemberInFederalDuty = membership.JustificationMemberInFederalDuty;
                membershipCandidate.RequirementsProfile = membership.RequirementsProfile;

                await _membershipCandidateRepository.CommitChanges();

                _logger.LogInformation("Updated membership candidate {MembershipCandidateId} with data from current membership", membershipCandidate.Id);
            }
        }
    }
}
