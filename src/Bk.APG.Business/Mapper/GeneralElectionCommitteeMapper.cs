using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Extensions;
using Bk.APG.Business.Models;
using Bk.APG.Common.Resources;

namespace Bk.APG.Business.Mapper;

public static class GeneralElectionCommitteeMapper
{
    public static GeneralElectionCommitteeFilterParameters ToGeneralElectionCommitteeFilterParameters(GeneralElectionCommitteeFilterParametersDto? filter)
    {
        return new GeneralElectionCommitteeFilterParameters
        {
            FreeText = filter?.FreeText,
            LevelIds = filter?.LevelIds,
            DepartmentIds = filter?.DepartmentIds,
            OfficeIds = filter?.OfficeIds,
            CommitteeTypeIds = filter?.CommitteeTypeIds,
            IsMarketOrientated = filter?.IsMarketOrientated,
            HasSupervisionDuty = filter?.HasSupervisionDuty,
            CommitteeIds = []
        };
    }

    public static GeneralElectionCommitteeExportFilterParameters ToGeneralElectionCommitteeExportFilterParameters(GeneralElectionCommitteeExportFilterParametersDto? filter)
    {
        return new GeneralElectionCommitteeExportFilterParameters
        {
            CorrespondenceLanguageIds = filter?.CorrespondenceLanguageIds,
            DepartmentIds = filter?.DepartmentIds,
            OfficeIds = filter?.OfficeIds,
            CommitteeTypeIds = filter?.CommitteeTypeIds,
            ElectionTypeIds = filter?.ElectionTypeIds,
            CommitteeIds = []
        };
    }

    public static GeneralElectionCommittee FromGeneralElectionCommitteeCreateDto(GeneralElectionCommitteeCreateDto createDto, string currentUserName)
    {
        return new GeneralElectionCommittee
        {
            BeginDate = createDto.BeginDate,
            EndDate = createDto.EndDate,
            DescriptionGerman = createDto.DescriptionGerman,
            DescriptionFrench = createDto.DescriptionFrench,
            DescriptionItalian = createDto.DescriptionItalian,
            DescriptionRomansh = createDto.DescriptionRomansh,
            CommitteeLevelId = createDto.LevelId,
            OfficeId = createDto.OfficeId,
            DepartmentId = createDto.DepartmentId,
            CommitteeTypeId = createDto.CommitteeTypeId,
            ReleaseGeneralElection = false,
            FederalLawEstablishment = createDto.FederalLawEstablishment,
            SupervisionDuty = createDto.SupervisionDuty,
            MarketOrientated = createDto.MarketOrientated,
            LegalFormId = createDto.LegalFormId,
            LegalBase = createDto.LegalBase,
            TermOfOfficeId = createDto.TermOfOfficeId,
            TermOfOfficeDateId = createDto.TermOfOfficeId == TermOfOffice.Period4YearsInGeneralElectionGuid ? TermOfOfficeDate.IndefiniteDurationGuid : TermOfOfficeDate.CurrentGeneralElectionGuid,
            MinimalMembers = createDto.MinimalMembers,
            MaximalMembers = createDto.MaximalMembers,
            AdditionalAuthorityMembers = createDto.AdditionalAuthorityMembers,
            LinkAuthorityWebsite = createDto.LinkAuthorityWebsite,
            LinkHomepageGerman = createDto.LinkHomepageGerman,
            LinkHomepageFrench = createDto.LinkHomepageFrench,
            LinkHomepageItalian = createDto.LinkHomepageItalian,
            LinkHomepageRomansh = createDto.LinkHomepageRomansh,
            Created = DateTime.UtcNow,
            CreatedBy = currentUserName,
            Modified = DateTime.UtcNow,
            ModifiedBy = currentUserName,
            IsDeleted = false,
            IsValidated = false,
        };
    }

    public static GeneralElectionCommitteeDetailDto ToGeneralElectionCommitteeDetailDto(GeneralElectionCommittee generalElectionCommittee)
    {
        var committeeDetailDto = new GeneralElectionCommitteeDetailDto
        {
            Id = generalElectionCommittee.Id,
            CommitteeId = generalElectionCommittee.CommitteeId,
            CommitteeNumber = generalElectionCommittee.Committee!.CommitteeNumber,
            Description = generalElectionCommittee.GetDescription(),
            DescriptionDe = generalElectionCommittee.DescriptionGerman,
            DescriptionFr = generalElectionCommittee.DescriptionFrench,
            DescriptionIt = generalElectionCommittee.DescriptionItalian,
            DescriptionRm = generalElectionCommittee.DescriptionRomansh,
            BeginDate = generalElectionCommittee.BeginDate,
            EndDate = generalElectionCommittee.EndDate,
            Department = generalElectionCommittee.Department!.GetText(),
            Office = generalElectionCommittee.Office!.GetText(),
            CommitteeLevel = generalElectionCommittee.CommitteeLevel!.GetText(),
            CommitteeType = generalElectionCommittee.CommitteeType!.GetText(),
            CommitteeTypeId = generalElectionCommittee.CommitteeTypeId,
            LegalForm = generalElectionCommittee.LegalForm?.GetText(),
            OldLegalForm = generalElectionCommittee.OldLegalForm,
            LegalBase = generalElectionCommittee.LegalBase,
            ReleaseGeneralElection = generalElectionCommittee.ReleaseGeneralElection,
            FederalLawEstablishment = generalElectionCommittee.FederalLawEstablishment,
            MarketOrientated = generalElectionCommittee.MarketOrientated,
            SupervisionDuty = generalElectionCommittee.SupervisionDuty,
            TermOfOffice = generalElectionCommittee.TermOfOffice!.GetText(),
            TermOfOfficeBeginDate = generalElectionCommittee.TermOfOfficeDate!.BeginDate,
            TermOfOfficeEndDate = generalElectionCommittee.TermOfOfficeDate!.EndDate,
            Period4YearsInGeneralElection = generalElectionCommittee.TermOfOfficeId == TermOfOffice.Period4YearsInGeneralElectionGuid,
            MinimalMembers = generalElectionCommittee.MinimalMembers,
            MaximalMembers = generalElectionCommittee.MaximalMembers,
            MembersCount = 0, // TODO BKDO-1031
            VacanciesGeneralElection = generalElectionCommittee.VacanciesGeneralElection,
            CalculatedVacancies = generalElectionCommittee.MinimalMembers - generalElectionCommittee.ActiveMemberCount > 0
                ? generalElectionCommittee.MinimalMembers - generalElectionCommittee.ActiveMemberCount
                : 0,
            AdditionalAuthorityMembers = generalElectionCommittee.AdditionalAuthorityMembers,
            LinkAuthorityWebsite = generalElectionCommittee.LinkAuthorityWebsite,
            RemarksBaseData = generalElectionCommittee.RemarksBaseData,
            RemarksBaseDataAdmin = generalElectionCommittee.RemarksBaseDataAdmin,
            IsDeleted = generalElectionCommittee.IsDeleted,
            ContactPoints = generalElectionCommittee.ContactPoints.Select(ContactPointMapper.ToContactPointDetailDto).ToList(),
            JustificationMembers = generalElectionCommittee.JustificationMembers,
            FederalInstitution = generalElectionCommittee.FederalInstitution,
            ExtraParliamentaryCommission = generalElectionCommittee.ExtraParliamentaryCommission,
            IsValidated = generalElectionCommittee.IsValidated,
            SelectionProcedure = generalElectionCommittee.SelectionProcedure,
            JustificationsNeedAttention = generalElectionCommittee.JustificationsNeedAttention,
            CanEditSelectionProcedure = generalElectionCommittee.CommitteeTypeId == CommitteeType.ManagementCommitteeGuid ||
                                        generalElectionCommittee.CommitteeTypeId == CommitteeType.FederalAgenciesCommitteeGuid,
            CandidateListState = generalElectionCommittee.CandidateListState!.GetText(),
            Quotas = generalElectionCommittee.GetQuotas(),
            Candidates = generalElectionCommittee.MembershipCandidates.Select(MembershipCandidateMapper.ToMembershipCandidateDetailDto)
        };

        return committeeDetailDto;
    }

    public static GeneralElectionCommitteeUpdateDto ToGeneralElectionCommitteeUpdateDto(GeneralElectionCommittee generalElectionCommitteeUpdate)
    {
        return new GeneralElectionCommitteeUpdateDto
        {
            Id = generalElectionCommitteeUpdate.Id,
            CommitteeId = generalElectionCommitteeUpdate.CommitteeId,
            CommitteeNumber = generalElectionCommitteeUpdate.Committee!.CommitteeNumber,
            BeginDate = generalElectionCommitteeUpdate.BeginDate,
            EndDate = generalElectionCommitteeUpdate.EndDate,
            CommitteeEndDate = generalElectionCommitteeUpdate.Committee.EndDate,
            DescriptionGerman = generalElectionCommitteeUpdate.DescriptionGerman,
            DescriptionFrench = generalElectionCommitteeUpdate.DescriptionFrench,
            DescriptionItalian = generalElectionCommitteeUpdate.DescriptionItalian,
            DescriptionRomansh = generalElectionCommitteeUpdate.DescriptionRomansh,
            LevelId = generalElectionCommitteeUpdate.CommitteeLevelId,
            OfficeId = generalElectionCommitteeUpdate.OfficeId,
            DepartmentId = generalElectionCommitteeUpdate.DepartmentId,
            CommitteeTypeId = generalElectionCommitteeUpdate.CommitteeTypeId,
            FederalLawEstablishment = generalElectionCommitteeUpdate.FederalLawEstablishment,
            SupervisionDuty = generalElectionCommitteeUpdate.SupervisionDuty,
            MarketOrientated = generalElectionCommitteeUpdate.MarketOrientated,
            LegalFormId = generalElectionCommitteeUpdate.LegalFormId,
            OldLegalForm = generalElectionCommitteeUpdate.OldLegalForm,
            LegalBase = generalElectionCommitteeUpdate.LegalBase,
            TermOfOfficeId = generalElectionCommitteeUpdate.TermOfOfficeId,
            MinimalMembers = generalElectionCommitteeUpdate.MinimalMembers,
            MaximalMembers = generalElectionCommitteeUpdate.MaximalMembers,
            AdditionalAuthorityMembers = generalElectionCommitteeUpdate.AdditionalAuthorityMembers,
            LinkAuthorityWebsite = generalElectionCommitteeUpdate.LinkAuthorityWebsite,
            LinkHomepageGerman = generalElectionCommitteeUpdate.LinkHomepageGerman,
            LinkHomepageFrench = generalElectionCommitteeUpdate.LinkHomepageFrench,
            LinkHomepageItalian = generalElectionCommitteeUpdate.LinkHomepageItalian,
            LinkHomepageRomansh = generalElectionCommitteeUpdate.LinkHomepageRomansh,
            FederalInstitution = generalElectionCommitteeUpdate.FederalInstitution,
            SelectionProcedure = generalElectionCommitteeUpdate.SelectionProcedure,
            MembersCount = 0, // TODO BKDO-1031
            RowVersion = generalElectionCommitteeUpdate.RowVersion
        };
    }

    public static GeneralElectionCommitteeListDto ToGeneralElectionCommitteeListDto(GeneralElectionCommittee generalElectionCommittee, CultureInfo cultureInfo)
    {
        return new GeneralElectionCommitteeListDto
        {
            Id = generalElectionCommittee.Id,
            CommitteeId = generalElectionCommittee.CommitteeId,
            Description = generalElectionCommittee.GetDescription(cultureInfo),
            Department = generalElectionCommittee.Department!.GetText(cultureInfo),
            Office = generalElectionCommittee.Office!.GetText(cultureInfo),
            CommitteeType = generalElectionCommittee.CommitteeType!.GetText(cultureInfo),
            Status = string.Empty,
            VacanciesGeneralElection = generalElectionCommittee.VacanciesGeneralElection,
            StatusProposal = string.Empty,
            IsMarketOrientated = generalElectionCommittee.MarketOrientated,
            HasSupervisionDuty = generalElectionCommittee.SupervisionDuty,
            Modified = generalElectionCommittee.Modified,
            ModifiedBy = generalElectionCommittee.ModifiedBy,
        };
    }

    public static GeneralElectionCommitteeJustificationUpdateDto ToGeneralElectionCommitteeJustificationUpdateDto(GeneralElectionCommittee generalElectionCommittee)
    {
        return new GeneralElectionCommitteeJustificationUpdateDto
        {
            Id = generalElectionCommittee.Id,
            JustificationMembers = generalElectionCommittee.JustificationMembers,
            IsJustificationGendersRequired = generalElectionCommittee.IsJustificationGendersRequired,
            JustificationGenders = generalElectionCommittee.JustificationGenders,
            MeasuresGenders = generalElectionCommittee.MeasuresGenders,
            IsJustificationLanguagesRequired = generalElectionCommittee.IsJustificationLanguagesRequired,
            JustificationLanguages = generalElectionCommittee.JustificationLanguages,
            MeasuresLanguages = generalElectionCommittee.MeasuresLanguages,
            SelectionProcedure = generalElectionCommittee.SelectionProcedure,
            CurrentMemberCount = generalElectionCommittee.ActiveMemberCount,
            CurrentGenderQuota = GenerateGenderQuotaString(generalElectionCommittee),
            CurrentLanguageQuota = GenerateLanguageQuotaString(generalElectionCommittee),
            RowVersion = generalElectionCommittee.RowVersion
        };
    }

    private static string GenerateGenderQuotaString(GeneralElectionCommittee generalElectionCommittee)
    {
        return $"{generalElectionCommittee.MaleQuota} % {BusinessTexts.Common_MaleAbbreviation}, {generalElectionCommittee.FemaleQuota} % {BusinessTexts.Common_FemaleAbbreviation}";
    }

    private static string GenerateLanguageQuotaString(GeneralElectionCommittee generalElectionCommittee)
    {
        var usePercentages = generalElectionCommittee.CommitteeType!.GermanThresholdPercentage is not null;

        return usePercentages
            ? $"{generalElectionCommittee.GermanQuota} % DE, {generalElectionCommittee.FrenchQuota} % FR, {generalElectionCommittee.ItalianQuota} % IT, {generalElectionCommittee.RomanshQuota} % RM"
            : $"{generalElectionCommittee.GermanCount} DE, {generalElectionCommittee.FrenchCount} FR, {generalElectionCommittee.ItalianCount} IT, {generalElectionCommittee.RomanshCount} RM";
    }
}
