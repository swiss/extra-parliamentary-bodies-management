using Bk.APG.Business.Dtos;
using Bk.APG.Business.Extensions;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Mapper;

public static class GeneralElectionMapper
{
    public static GeneralElectionCommittee FromCommitteeToGeneralElectionCommittee(Committee committee, string currentUserName)
    {
        return new GeneralElectionCommittee
        {
            CommitteeId = committee.Id,
            BeginDate = committee.BeginDate,
            EndDate = committee.EndDate,
            DescriptionGerman = committee.DescriptionGerman,
            DescriptionFrench = committee.DescriptionFrench,
            DescriptionItalian = committee.DescriptionItalian,
            DescriptionRomansh = committee.DescriptionRomansh,
            CommitteeLevelId = committee.CommitteeLevelId,
            OfficeId = committee.OfficeId,
            DepartmentId = committee.DepartmentId,
            CommitteeTypeId = committee.CommitteeTypeId,
            ReleaseGeneralElection = committee.ReleaseGeneralElection is not null && (bool)committee.ReleaseGeneralElection,
            FederalLawEstablishment = committee.FederalLawEstablishment,
            SupervisionDuty = committee.SupervisionDuty,
            MarketOrientated = committee.MarketOrientated,
            LegalFormId = committee.LegalFormId,
            OldLegalForm = committee.OldLegalForm,
            LegalBase = committee.LegalBase,
            TermOfOfficeId = committee.TermOfOfficeId,
            TermOfOfficeDateId = TermOfOfficeDate.NextGeneralElectionGuid,
            MinimalMembers = committee.MinimalMembers,
            MaximalMembers = committee.MaximalMembers,
            AdditionalAuthorityMembers = committee.AdditionalAuthorityMembers,
            LinkAuthorityWebsite = committee.LinkAuthorityWebsite,
            LinkHomepageGerman = committee.LinkHomepageGerman,
            LinkHomepageFrench = committee.LinkHomepageFrench,
            LinkHomepageItalian = committee.LinkHomepageItalian,
            LinkHomepageRomansh = committee.LinkHomepageRomansh,
            // set to null on purpose!
            VacanciesGeneralElection = null,
            RemarksBaseData = committee.RemarksBaseData,
            RemarksBaseDataAdmin = committee.RemarksBaseDataAdmin,
            Created = DateTime.UtcNow,
            CreatedBy = currentUserName,
            Modified = DateTime.UtcNow,
            ModifiedBy = currentUserName,
            IsDeleted = false,
            IsValidated = false,
            CandidateListStateId = CandidateListState.Draft,
            AssignedToRole = "admin"
        };
    }

    public static Committee FromGeneralElectionCommitteeToCommittee(GeneralElectionCommittee committee)
    {
        return new Committee
        {
            // we want it to be the original CommitteeID!
            Id = committee.CommitteeId,
            BeginDate = committee.BeginDate,
            EndDate = committee.EndDate,
            DescriptionGerman = committee.DescriptionGerman,
            DescriptionFrench = committee.DescriptionFrench,
            DescriptionItalian = committee.DescriptionItalian,
            DescriptionRomansh = committee.DescriptionRomansh,
            CommitteeLevelId = committee.CommitteeLevelId,
            CommitteeLevel = committee.CommitteeLevel,
            OfficeId = committee.OfficeId,
            Office = committee.Office,
            DepartmentId = committee.DepartmentId,
            Department = committee.Department,
            CommitteeTypeId = committee.CommitteeTypeId,
            CommitteeType = committee.CommitteeType,
            ReleaseGeneralElection = committee.ReleaseGeneralElection is not null && (bool)committee.ReleaseGeneralElection,
            FederalLawEstablishment = committee.FederalLawEstablishment,
            FederalInstitution = committee.FederalInstitution,
            SupervisionDuty = committee.SupervisionDuty,
            MarketOrientated = committee.MarketOrientated,
            LegalFormId = committee.LegalFormId,
            OldLegalForm = committee.OldLegalForm,
            LegalBase = committee.LegalBase,
            TermOfOfficeId = committee.TermOfOfficeId,
            TermOfOffice = committee.TermOfOffice,
            TermOfOfficeDateId = TermOfOfficeDate.NextGeneralElectionGuid,
            MinimalMembers = committee.MinimalMembers,
            MaximalMembers = committee.MaximalMembers,
            AdditionalAuthorityMembers = committee.AdditionalAuthorityMembers,
            LinkAuthorityWebsite = committee.LinkAuthorityWebsite,
            VacanciesGeneralElection = committee.VacanciesGeneralElection,
            RemarksBaseData = committee.RemarksBaseData,
            RemarksBaseDataAdmin = committee.RemarksBaseDataAdmin,
            Created = committee.Created,
            CreatedBy = committee.CreatedBy,
            Modified = committee.Modified,
            ModifiedBy = committee.ModifiedBy,
            Memberships = committee.MembershipCandidates.Select(FromMembershipCandidateToMembership).ToList(),
            IsDeleted = false
        };
    }

    public static MembershipCandidate FromMembershipAndPersonToMembershipCandidate(Membership membership, Guid generalElectionCommitteeId, string currentUserName, DateOnly termOfOfficeStartDate, DateOnly termOfOfficeEndDate)
    {
        ArgumentNullException.ThrowIfNull(membership.Person);
        ArgumentNullException.ThrowIfNull(membership.Person.Surname);
        ArgumentNullException.ThrowIfNull(membership.Person.GivenName);
        ArgumentNullException.ThrowIfNull(membership.Person.BirthYear);
        ArgumentNullException.ThrowIfNull(membership.Person.LanguageId);
        ArgumentNullException.ThrowIfNull(membership.Person.GenderId);

        return new MembershipCandidate
        {
            GeneralElectionCommitteeId = generalElectionCommitteeId,
            PersonId = membership.Person.Id,
            Surname = membership.Person.Surname,
            GivenName = membership.Person.GivenName,
            BirthYear = membership.Person.BirthYear,
            LanguageId = membership.Person.LanguageId,
            GenderId = membership.Person.GenderId,
            MaximumEmploymentLevel = membership.MaximumEmploymentLevel,
            BeginDate = termOfOfficeStartDate,
            EndDate = termOfOfficeEndDate,
            ElectionTypeId = ElectionType.ReElectionGuid,
            FunctionId = membership.FunctionId,
            ElectionOfficeId = membership.ElectionOfficeId,
            OldMembershipAddition = null,
            MembershipAdditionId = membership.MembershipAdditionId,
            MembershipId = membership.Id,
            JustificationLongerDuty = membership.JustificationLongerDuty,
            // Justifications for shorter duration are explicitly empty, must be rewritten for every GE!
            JustificationShorterDuty = string.Empty,
            JustificationMemberInFederalDuty = membership.JustificationMemberInFederalDuty,
            JustificationMemberInFederalAssembly = membership.JustificationMemberInFederalAssembly,
            RequirementsProfile = string.Empty,
            Remarks = membership.Remarks,
            RemarksStatus = membership.RemarksStatus,
            Created = DateTime.UtcNow,
            CreatedBy = currentUserName,
            Modified = DateTime.UtcNow,
            ModifiedBy = currentUserName,
            InCorrelationWithFederalDuty = membership.InCorrelationWithFederalDuty,
            IsDeleted = false,
            IsSelected = false,
        };
    }

    public static Membership FromMembershipCandidateToMembership(MembershipCandidate membershipCandidate)
    {
        return new Membership
        {
            PersonId = membershipCandidate.Person != null ? membershipCandidate.Person.Id : Guid.Empty,
            Person = membershipCandidate.Person,
            MaximumEmploymentLevel = membershipCandidate.MaximumEmploymentLevel,
            BeginDate = membershipCandidate.BeginDate,
            EndDate = membershipCandidate.EndDate,
            ElectionType = membershipCandidate.ElectionType,
            ElectionTypeId = ElectionType.ReElectionGuid,
            Function = membershipCandidate.Function,
            FunctionId = membershipCandidate.FunctionId,
            ElectionOfficeId = membershipCandidate.ElectionOfficeId,
            ElectionOffice = membershipCandidate.ElectionOffice,
            OldMembershipAddition = null,
            MembershipAdditionId = membershipCandidate.MembershipAdditionId,
            JustificationLongerDuty = membershipCandidate.JustificationLongerDuty,
            JustificationShorterDuty = membershipCandidate.JustificationShorterDuty,
            JustificationMemberInFederalDuty = membershipCandidate.JustificationMemberInFederalDuty,
            JustificationMemberInFederalAssembly = membershipCandidate.JustificationMemberInFederalAssembly,
            RequirementsProfile = membershipCandidate.RequirementsProfile,
            Remarks = membershipCandidate.Remarks,
            RemarksStatus = membershipCandidate.RemarksStatus,
            Created = membershipCandidate.Created,
            CreatedBy = membershipCandidate.CreatedBy,
            Modified = membershipCandidate.Modified,
            ModifiedBy = membershipCandidate.ModifiedBy,
            InCorrelationWithFederalDuty = membershipCandidate.InCorrelationWithFederalDuty,
            IsDeleted = false
        };
    }

    public static MembershipCandidateMirrorDto ToMembershipCandidateMirrorDto(Membership membership)
    {
        return new MembershipCandidateMirrorDto
        {
            MaximumEmploymentLevel = membership.MaximumEmploymentLevel,
            ElectionTypeId = ElectionType.ReElectionGuid,
            FunctionId = membership.FunctionId,
            ElectionOfficeId = membership.ElectionOfficeId,
            MembershipAdditionId = membership.MembershipAdditionId,
            Remarks = membership.Remarks,
            RemarksStatus = membership.RemarksStatus,
            Modified = membership.Modified,
            ModifiedBy = membership.ModifiedBy,
            InCorrelationWithFederalDuty = membership.InCorrelationWithFederalDuty,
            JustificationLongerDuty = membership.JustificationLongerDuty,
            JustificationMemberInFederalAssembly = membership.JustificationMemberInFederalAssembly,
            JustificationMemberInFederalDuty = membership.JustificationMemberInFederalDuty,
        };
    }

    public static CommitteeMemberDto ToCommitteeMemberDto(MembershipCandidate membershipCandidate)
    {
        return new CommitteeMemberDto
        {
            Id = membershipCandidate.Id,
            PersonId = membershipCandidate.PersonId ?? Guid.Empty,
            Surname = membershipCandidate.Person?.Surname ?? membershipCandidate.Surname,
            GivenName = membershipCandidate.Person?.GivenName ?? membershipCandidate.GivenName,
            Gender = membershipCandidate.Person?.Gender?.GetText() ?? membershipCandidate.Gender?.GetText() ?? string.Empty,
            Language = membershipCandidate.Person?.Language?.GetText() ?? membershipCandidate.Language?.GetText() ?? string.Empty,
            EmploymentLevel = membershipCandidate.GetEmploymentLevel(),
            Function = GetFunction(),
            BeginDate = membershipCandidate.BeginDate,
            EndDate = membershipCandidate.EndDate,
            ElectionType = membershipCandidate.ElectionType?.GetText() ?? string.Empty,
            HasMembershipAddition = membershipCandidate.MembershipAddition is not null,
            IsActive = true,
            IsFuture = false,
            NeedsAttention = membershipCandidate.HasMembershipValidationIssues
        };

        string GetFunction()
        {
            if (membershipCandidate.Person is not null)
            {
                return (membershipCandidate.Person.GenderId == Gender.FemaleGuid ? membershipCandidate.Function?.GetFemaleText() : membershipCandidate.Function?.GetText()) ?? string.Empty;
            }

            return (membershipCandidate.GenderId == Gender.FemaleGuid ? membershipCandidate.Function?.GetFemaleText() : string.Empty) ?? string.Empty;
        }
    }
}
