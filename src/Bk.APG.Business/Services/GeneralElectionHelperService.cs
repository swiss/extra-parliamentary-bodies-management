using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Common.Resources;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Business.Services;

public class GeneralElectionHelperService : IGeneralElectionHelperService
{
    private readonly IMembershipCandidateRepository _membershipCandidateRepository;
    private readonly IMembershipCandidateLogMessageRepository _membershipCandidateLogMessageRepository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly IGeneralElectionCommitteeRepository _generalElectionCommitteeRepository;
    private readonly ILogger<GeneralElectionHelperService> _logger;

    public GeneralElectionHelperService(
        IMembershipCandidateRepository membershipCandidateRepository,
        IMembershipCandidateLogMessageRepository membershipCandidateLogMessageRepository,
        IMembershipRepository membershipRepository,
        IGeneralElectionCommitteeRepository generalElectionCommitteeRepository,
        ILogger<GeneralElectionHelperService> logger)
    {
        _membershipCandidateRepository = membershipCandidateRepository;
        _membershipCandidateLogMessageRepository = membershipCandidateLogMessageRepository;
        _membershipRepository = membershipRepository;
        _generalElectionCommitteeRepository = generalElectionCommitteeRepository;
        _logger = logger;
    }

    public async Task CreateNewMembershipCandidate(Membership membership, string userName)
    {
        _logger.LogInformation("Create new membership candidate for general election committee {CommitteeId}", membership.CommitteeId);

        var generalElectionCommittee = await _generalElectionCommitteeRepository.GetByCommitteeId(membership.CommitteeId);
        var termOfOfficeDate = generalElectionCommittee.TermOfOfficeDate;

        var membershipCandidate = GeneralElectionMapper.FromMembershipAndPersonToMembershipCandidate(membership, generalElectionCommittee.Id, userName, termOfOfficeDate!.BeginDate, (DateOnly)termOfOfficeDate.EndDate!);

        // Check for maximum duration
        var newEndDate = await CheckMembershipCandidateSpecialCases(membershipCandidate, generalElectionCommittee, membership.Person!.FederalDuty, (DateOnly)termOfOfficeDate.EndDate!);

        if (termOfOfficeDate.BeginDate != newEndDate)
        {
            membershipCandidate.EndDate = newEndDate;
            await _membershipCandidateRepository.Create(membershipCandidate);
            _logger.LogInformation("New membership candidate created {MembershipCandidateId}", membershipCandidate.Id);
        }
        else
        {
            // Here we write the new log table, saying that Member XX has not been transferred to new period
            var personInfo = $"PersonId {membershipCandidate.PersonId}, {membershipCandidate.GivenName} {membershipCandidate.Surname}, {membershipCandidate.BirthYear}";
            var errorText = BusinessTexts.GeneralElectionMaximumDurationExceeded + personInfo;

            var dto = MembershipCandidateLogMessageMapper.ToMembershipCandidateLogMessage(generalElectionCommittee.Id, (Guid)membershipCandidate.PersonId!, errorText, userName);
            await _membershipCandidateLogMessageRepository.Create(dto);
            _logger.LogInformation("Membership candidate for person {PersonId} not created because maximum duration exceeded", membershipCandidate.PersonId);
        }
    }

    public async Task<DateOnly> CheckMembershipCandidateSpecialCases(
         MembershipCandidate membershipCandidate,
         GeneralElectionCommittee committee,
         bool isPersonInFederalDuty,
         DateOnly termOfOfficeEndDate)
    {
        var correctEndDate = membershipCandidate.EndDate;

        if ((committee.CommitteeTypeId == CommitteeType.AuthoritiesCommissionGuid || committee.CommitteeTypeId == CommitteeType.AdministrationCommissionGuid) &&
            (!membershipCandidate.InCorrelationWithFederalDuty || (membershipCandidate.InCorrelationWithFederalDuty && isPersonInFederalDuty)))
        {
            // if all these conditions match, we have to check for the maximum duration of 16 years. So if current duration is smaller than 12 years, membership will continue.
            // If bigger than 12 years, the end date has to be set accordingly.
            var personMemberships = await _membershipRepository.GetAllMembershipsForCommitteeAndPerson(committee.CommitteeId, (Guid)membershipCandidate.PersonId!);

            var totalDays = 0;

            foreach (var membership in personMemberships)
            {
                totalDays += membership.EndDate.DayNumber - membership.BeginDate.DayNumber;
            }

            var currentYears = (int)Math.Round((double)totalDays / 365);

            if (currentYears is >= 13 and < 16)
            {
                // 16 -13 = 3 / 4 - 3
                var allowedYears = 16 - currentYears;

                correctEndDate = termOfOfficeEndDate.AddYears((4 - allowedYears) * -1);
            }
            else if (currentYears is >= 16)
            {
                correctEndDate = membershipCandidate.BeginDate;
            }
        }

        return correctEndDate;
    }

    public async Task MirrorOrDeleteMembershipForGeneralElection(Membership membership, bool deleteCandidate)
    {
        _logger.LogInformation("Mirror membership {MembershipId} for general election", membership.Id);

        var membershipCandidate = await _membershipCandidateRepository.GetByMembershipIdForUpdate(membership.Id);

        if (membershipCandidate != null)
        {
            if (membershipCandidate.GeneralElectionCommittee?.IsValidated == true)
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
