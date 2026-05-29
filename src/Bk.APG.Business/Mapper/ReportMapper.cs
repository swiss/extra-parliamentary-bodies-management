using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Mapper;

public static class ReportMapper
{
    public static ReportGeneralElectionCommitteeDto FromGeneralElectionCommitteeToReportGeneralElectionCommitteeDto(GeneralElectionCommittee committee)
    {
        ArgumentNullException.ThrowIfNull(committee);

        return new ReportGeneralElectionCommitteeDto
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
            SelfOrganized = committee.SelfOrganized,
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
            SelectionProcedure = committee.SelectionProcedure,
            LinkHomepageGerman = committee.LinkHomepageGerman,
            LinkHomepageFrench = committee.LinkHomepageFrench,
            LinkHomepageItalian = committee.LinkHomepageItalian,
            LinkHomepageRomansh = committee.LinkHomepageRomansh,
            JustificationGenders = committee.JustificationGenders,
            JustificationLanguages = committee.JustificationLanguages,
            JustificationMembers = committee.JustificationMembers,
            MeasuresGenders = committee.MeasuresGenders,
            MeasuresLanguages = committee.MeasuresLanguages,
            Memberships = committee.MembershipCandidates.Select(FromMembershipCandidateToReportMembershipDto).ToList(),
            IsDeleted = false,
            IsValidated = committee.IsValidated,
            CandidateListStateId = committee.CandidateListStateId,
        };
    }

    public static ReportGeneralElectionCommitteeDto FromCommitteeToReportGeneralElectionCommitteeDto(Committee committee)
    {
        ArgumentNullException.ThrowIfNull(committee);

        return new ReportGeneralElectionCommitteeDto
        {
            Id = committee.Id,
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
            SelfOrganized = committee.SelfOrganized,
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
            LinkHomepageGerman = committee.LinkHomepageGerman,
            LinkHomepageFrench = committee.LinkHomepageFrench,
            LinkHomepageItalian = committee.LinkHomepageItalian,
            LinkHomepageRomansh = committee.LinkHomepageRomansh,
            JustificationGenders = committee.JustificationGenders,
            JustificationLanguages = committee.JustificationLanguages,
            JustificationMembers = committee.JustificationMembers,
            MeasuresGenders = committee.MeasuresGenders,
            MeasuresLanguages = committee.MeasuresLanguages,
            MembershipAdditionsInGeneralElection = committee.MembershipAdditionsInGeneralElection,
            SelectionProcedure = string.Empty,
            Memberships = committee.Memberships.Count > 0 ? committee.Memberships.Select(m => FromMembershipToReportMembershipDto(m)).ToList() : new List<ReportGeneralElectionMembershipDto>(),
            IsDeleted = false,
            IsValidated = true,
        };
    }

    public static ReportGeneralElectionMembershipDto FromMembershipCandidateToReportMembershipDto(MembershipCandidate membershipCandidate)
    {
        ArgumentNullException.ThrowIfNull(membershipCandidate);

        return new ReportGeneralElectionMembershipDto
        {
            Id = membershipCandidate.Id,
            PersonId = membershipCandidate.Person?.Id ?? Guid.Empty,
            Person = membershipCandidate.Person,
            Surname = membershipCandidate.Surname,
            GivenName = membershipCandidate.GivenName,
            BirthYear = membershipCandidate.BirthYear,
            GenderId = membershipCandidate.GenderId,
            LanguageId = membershipCandidate.LanguageId,
            MaximumEmploymentLevel = membershipCandidate.MaximumEmploymentLevel,
            BeginDate = membershipCandidate.BeginDate,
            EndDate = membershipCandidate.EndDate,
            ElectionType = membershipCandidate.ElectionType,
            ElectionTypeId = membershipCandidate.ElectionTypeId,
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
            InCorrelationWithFederalDuty = membershipCandidate.InCorrelationWithFederalDuty,
            IsDeleted = false,
            IsSelected = membershipCandidate.IsSelected,
            EstimatedTermOfOffice = membershipCandidate.EstimatedTermOfOffice
        };
    }

    public static ReportGeneralElectionMembershipDto FromMembershipToReportMembershipDto(Membership membership)
    {
        ArgumentNullException.ThrowIfNull(membership);

        return new ReportGeneralElectionMembershipDto
        {
            PersonId = membership.Person != null ? membership.Person.Id : Guid.Empty,
            Person = membership.Person,
            Surname = membership.Person != null ? membership.Person.Surname : string.Empty,
            GivenName = membership.Person != null ? membership.Person.GivenName : string.Empty,
            BirthYear = membership.Person != null ? membership.Person.BirthYear : 0,
            GenderId = membership.Person != null ? membership.Person.GenderId : Guid.Empty,
            LanguageId = membership.Person != null ? membership.Person.LanguageId : Guid.Empty,
            MaximumEmploymentLevel = membership.MaximumEmploymentLevel,
            BeginDate = membership.BeginDate,
            EndDate = membership.EndDate,
            ElectionType = membership.ElectionType,
            ElectionTypeId = membership.ElectionTypeId,
            Function = membership.Function,
            FunctionId = membership.FunctionId,
            ElectionOfficeId = membership.ElectionOfficeId,
            ElectionOffice = membership.ElectionOffice,
            OldMembershipAddition = null,
            MembershipAdditionId = membership.MembershipAdditionId,
            JustificationLongerDuty = membership.JustificationLongerDuty,
            JustificationShorterDuty = membership.JustificationShorterDuty,
            JustificationMemberInFederalDuty = membership.JustificationMemberInFederalDuty,
            JustificationMemberInFederalAssembly = membership.JustificationMemberInFederalAssembly,
            RequirementsProfile = membership.RequirementsProfile,
            Remarks = membership.Remarks,
            RemarksStatus = membership.RemarksStatus,
            InCorrelationWithFederalDuty = membership.InCorrelationWithFederalDuty,
            IsDeleted = false,
            IsSelected = true,
            EstimatedTermOfOffice = 0
        };
    }
}
