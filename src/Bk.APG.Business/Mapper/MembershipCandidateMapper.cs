using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Mapper;

public static class MembershipCandidateMapper
{
    public static MembershipCandidateDetailDto ToMembershipCandidateDetailDto(MembershipCandidate membershipCandidate)
    {
        ArgumentNullException.ThrowIfNull(membershipCandidate);

        return new MembershipCandidateDetailDto
        {
            Id = membershipCandidate.Id,
            PersonId = membershipCandidate.PersonId,
            Surname = membershipCandidate.Person is not null ? membershipCandidate.Person.Surname : membershipCandidate.Surname,
            GivenName = membershipCandidate.Person is not null ? membershipCandidate.Person.GivenName : membershipCandidate.GivenName,
            BirthYear = membershipCandidate.Person is not null ? membershipCandidate.Person.BirthYear : membershipCandidate.BirthYear,
            Language = membershipCandidate.Person is not null ? membershipCandidate.Person.Language!.GetText() : membershipCandidate.Language!.GetText(),
            Gender = membershipCandidate.Person is not null ? membershipCandidate.Person.Gender!.GetText() : membershipCandidate.Gender!.GetText(),
            GenderId = membershipCandidate.Person is not null ? membershipCandidate.Person.Gender!.Id : membershipCandidate.Gender!.Id,
            Function = membershipCandidate.FunctionName,
            FunctionId = membershipCandidate.FunctionId,
            BeginDate = membershipCandidate.BeginDate,
            EndDate = membershipCandidate.EndDate,
            ElectionType = membershipCandidate.ElectionType!.GetText(),
            ElectionTypeId = membershipCandidate.ElectionTypeId,
            MembershipAddition = membershipCandidate.MembershipAddition?.GetText() ?? string.Empty,
            Remarks = membershipCandidate.Remarks,
            RemarksStatus = membershipCandidate.RemarksStatus,
            NeedsAttention = membershipCandidate.NeedsAttention,
            IsSelected = membershipCandidate.IsSelected,
            EstimatedTermOfOffice = membershipCandidate.EstimatedTermOfOffice,
        };
    }

    public static MembershipCandidateUpdateDto ToMembershipCandidateUpdateDto(MembershipCandidate membershipCandidate)
    {
        ArgumentNullException.ThrowIfNull(membershipCandidate);

        return new MembershipCandidateUpdateDto
        {
            Id = membershipCandidate.Id,
            PersonId = membershipCandidate.PersonId,
            Surname = membershipCandidate.Surname,
            GivenName = membershipCandidate.GivenName,
            BirthYear = membershipCandidate.BirthYear,
            GenderId = membershipCandidate.GenderId,
            LanguageId = membershipCandidate.LanguageId,
            MaximumEmploymentLevel = membershipCandidate.MaximumEmploymentLevel,
            BeginDate = membershipCandidate.BeginDate,
            EndDate = membershipCandidate.EndDate,
            ElectionTypeId = membershipCandidate.ElectionTypeId,
            FunctionId = membershipCandidate.FunctionId,
            ElectionOfficeId = membershipCandidate.ElectionOfficeId,
            MembershipAdditionId = membershipCandidate.MembershipAdditionId,
            InCorrelationWithFederalDuty = membershipCandidate.InCorrelationWithFederalDuty,
            JustificationLongerDuty = membershipCandidate.JustificationLongerDuty,
            JustificationShorterDuty = membershipCandidate.JustificationShorterDuty,
            JustificationMemberInFederalDuty = membershipCandidate.JustificationMemberInFederalDuty,
            JustificationMemberInFederalAssembly = membershipCandidate.JustificationMemberInFederalAssembly,
            RequirementsProfile = membershipCandidate.RequirementsProfile,
            RowVersion = membershipCandidate.RowVersion,
            EstimatedTermOfOffice = membershipCandidate.EstimatedTermOfOffice,
            CurrentTermOfOffice = membershipCandidate.CurrentTermOfOffice,
            NeedsLongerDutyJustification = membershipCandidate.NeedsLongerDutyJustification,
            NeedsShorterDutyJustification = membershipCandidate.NeedsShorterDutyJustification,
            NeedsFederalDutyJustification = membershipCandidate.NeedsFederalDutyJustification,
            NeedsFederalAssemblyJustification = membershipCandidate.NeedsFederalAssemblyJustification,
            NeedsRequirementsProfile = membershipCandidate.NeedsRequirementsProfile,
            MaximumDurationExceeded = membershipCandidate.MaximumDurationExceeded,
            HasFederalAssemblyAuthoritiesCommissionConflict = membershipCandidate.HasFederalAssemblyAuthoritiesCommissionConflict
        };
    }
}
