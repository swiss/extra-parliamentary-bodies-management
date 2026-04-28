using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Common.Resources;
using Bk.APG.CrossCutting;
using Swiss.FCh.Cube.Dimension.Model;
using Swiss.FCh.Cube.RawData.Model;

namespace Bk.APG.Business.Mapper;

public static class CommitteeMapper
{
    public static CommitteeListDto ToCommitteeListDto(Committee committee, CultureInfo cultureInfo)
    {
        ArgumentNullException.ThrowIfNull(committee);

        return new CommitteeListDto
        {
            Id = committee.Id,
            CommitteeId = committee.CommitteeNumber,
            Description = committee.GetDescription(cultureInfo),
            Level = committee.CommitteeLevel!.GetText(cultureInfo),
            Department = committee.Department!.GetText(cultureInfo),
            Office = committee.Office!.GetText(cultureInfo),
            CommitteeType = committee.CommitteeType!.GetText(cultureInfo),
            Term = committee.TermOfOffice!.GetText(cultureInfo),
            IsActive = committee.IsActive,
            IsMarketOrientated = committee.MarketOrientated,
            HasSupervisionDuty = committee.SupervisionDuty,
            NeedsAttention = committee.NeedsAttention
        };
    }

    public static Committee FromCommitteeCreateDto(CommitteeCreateDto createDto, string currentUserName)
    {
        ArgumentNullException.ThrowIfNull(createDto);

        return new Committee
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
            FederalLawEstablishment = createDto.FederalLawEstablishment,
            FederalInstitution = createDto.FederalInstitution,
            SelfOrganized = createDto.SelfOrganized,
            SupervisionDuty = createDto.SupervisionDuty,
            MarketOrientated = createDto.MarketOrientated,
            LegalFormId = createDto.LegalFormId,
            LegalBase = createDto.LegalBase,
            TermOfOfficeId = createDto.TermOfOfficeId,
            TermOfOfficeDateId = createDto.TermOfOfficeId == TermOfOffice.Period4YearsInGeneralElectionGuid ? TermOfOfficeDate.CurrentGeneralElectionGuid : TermOfOfficeDate.IndefiniteDurationGuid,
            MinimalMembers = createDto.MinimalMembers,
            MaximalMembers = createDto.MaximalMembers,
            AdditionalAuthorityMembers = createDto.AdditionalAuthorityMembers,
            LinkAuthorityWebsite = createDto.LinkAuthorityWebsite,
            LinkHomepageGerman = createDto.LinkHomepageGerman,
            LinkHomepageFrench = createDto.LinkHomepageFrench,
            LinkHomepageItalian = createDto.LinkHomepageItalian,
            LinkHomepageRomansh = createDto.LinkHomepageRomansh,
            VacanciesGeneralElection = createDto.VacanciesInGeneralElection,
            Created = DateTime.UtcNow,
            CreatedBy = currentUserName,
            Modified = DateTime.UtcNow,
            ModifiedBy = currentUserName,
            IsDeleted = false
        };
    }

    public static CommitteeDetailDto ToCommitteeDetailDto(Committee committee)
    {
        ArgumentNullException.ThrowIfNull(committee);

        var activeMembers = committee.Memberships
            .Where(x => x is { IsActive: true, IsDeleted: false })
            .ToArray();
        var activeMembersCount = activeMembers.Length;
        var committeeType = committee.CommitteeType!;
        var isPercentageBased = committeeType.GermanThresholdPercentage is not null;

        var committeeDetailDto = new CommitteeDetailDto
        {
            Id = committee.Id,
            CommitteeNumber = committee.CommitteeNumber,
            Description = committee.GetDescription(),
            DescriptionDe = committee.DescriptionGerman,
            DescriptionFr = committee.DescriptionFrench,
            DescriptionIt = committee.DescriptionItalian,
            DescriptionRm = committee.DescriptionRomansh,
            BeginDate = committee.BeginDate,
            EndDate = committee.EndDate,
            Department = committee.Department!.GetText(),
            Office = committee.Office!.GetText(),
            CommitteeLevel = committee.CommitteeLevel!.GetText(),
            CommitteeType = committee.CommitteeType!.GetText(),
            CommitteeTypeId = committee.CommitteeTypeId,
            LegalForm = committee.LegalForm?.GetText(),
            OldLegalForm = committee.OldLegalForm,
            LegalBase = committee.LegalBase,
            FederalLawEstablishment = committee.FederalLawEstablishment,
            MarketOrientated = committee.MarketOrientated,
            SupervisionDuty = committee.SupervisionDuty,
            TermOfOffice = committee.TermOfOffice!.GetText(),
            TermOfOfficeBeginDate = committee.TermOfOfficeDate!.BeginDate,
            TermOfOfficeEndDate = committee.TermOfOfficeDate!.EndDate,
            Period4YearsInGeneralElection = committee.TermOfOfficeId == TermOfOffice.Period4YearsInGeneralElectionGuid,
            MembersCount = activeMembersCount,
            MinimalMembers = committee.MinimalMembers,
            MaximalMembers = committee.MaximalMembers,
            VacanciesGeneralElection = committee.VacanciesGeneralElection,
            AdditionalAuthorityMembers = committee.AdditionalAuthorityMembers,
            LinkAuthorityWebsite = committee.LinkAuthorityWebsite,
            RemarksBaseData = committee.RemarksBaseData,
            RemarksBaseDataAdmin = committee.RemarksBaseDataAdmin,
            IsDeleted = committee.IsDeleted,
            ContactPoints = committee.ContactPoints.Select(ContactPointMapper.ToContactPointDetailDto).ToList(),
            IsActive = committee.IsActive,
            CanCreateMembership = committee.CanCreateMembership,
            JustificationMembers = committee.JustificationMembers,
            FemaleThreshold = committee.CommitteeType!.FemaleThreshold,
            FemaleQuota = activeMembersCount > 0 ? (double)activeMembers.Count(x => x.Person!.Gender!.Uri == Gender.Female) / activeMembersCount * 100 : 0,
            MaleThreshold = committee.CommitteeType!.MaleThreshold,
            MaleQuota = activeMembersCount > 0 ? (double)activeMembers.Count(x => x.Person!.Gender!.Uri == Gender.Male) / activeMembersCount * 100 : 0,
            JustificationGenders = committee.JustificationGenders,
            MeasuresGenders = committee.MeasuresGenders,
            IsPercentageBased = isPercentageBased,
            GermanThreshold = (isPercentageBased ? committeeType.GermanThresholdPercentage : committeeType.GermanMinimalThreshold) ?? 0,
            GermanQuota = isPercentageBased ? committee.GermanQuota : committee.GermanCount,
            FrenchThreshold = (isPercentageBased ? committeeType.FrenchThresholdPercentage : committeeType.FrenchMinimalThreshold) ?? 0,
            FrenchQuota = isPercentageBased ? committee.FrenchQuota : committee.FrenchCount,
            ItalianThreshold = (isPercentageBased ? committeeType.ItalianThresholdPercentage : committeeType.ItalianMinimalThreshold) ?? 0,
            ItalianQuota = isPercentageBased ? committee.ItalianQuota : committee.ItalianCount,
            RomanshThreshold = (isPercentageBased ? committeeType.RomanshThresholdPercentage : committeeType.RomanshMinimalThreshold) ?? 0,
            RomanshQuota = isPercentageBased ? committee.RomanshQuota : committee.RomanshCount,
            JustificationLanguages = committee.JustificationLanguages,
            MeasuresLanguages = committee.MeasuresLanguages,
            FederalInstitution = committee.FederalInstitution,
            SelfOrganized = committee.SelfOrganized,
            ExtraParliamentaryCommission = committee.ExtraParliamentaryCommission,
            VacanciesInCurrentTermOfOffice = committee.VacanciesInCurrentTermOfOffice,
            NeedsAttentionShorterDuty = committee.NeedsAttentionShorterDuty,
            NeedsAttentionLongerDuty = committee.NeedsAttentionLongerDuty,
            NeedsAttentionFederalDuty = committee.NeedsAttentionFederalDuty,
            NeedsAttentionFederalAssembly = committee.NeedsAttentionFederalAssembly,
            NeedsAttentionNoMembers = committee.NeedsAttentionNoMembers,
            NeedsAttentionAboveMaxMembers = committee.NeedsAttentionAboveMaxMembers,
            NeedsAttentionDataProtectionOfficer = committee.NeedsAttentionDataProtectionOfficer,
            NeedsAttentionSecretariat = committee.NeedsAttentionSecretariat,
            NeedsAttentionBasicData = committee.NeedsAttentionBasicData,
            NeedsAttentionMembershipExpired = committee.NeedsAttentionMembershipExpired,
            NeedsAttentionMembershipInterestOrOccupation = committee.NeedsAttentionMembershipInterestOrOccupation,
            NeedsAttentionRequirementsProfile = committee.NeedsAttentionRequirementsProfile,
            FutureGeneralElectionCommittee = committee.FutureGeneralElectionCommittee
        };
        return committeeDetailDto;
    }

    public static CommitteeFilterParameters? ToCommitteeFilterParameters(CommitteeFilterParametersDto? filter)
    {
        if (filter is null)
        {
            return null;
        }

        return new CommitteeFilterParameters
        {
            FreeText = filter.FreeText,
            LevelIds = filter.LevelIds,
            DepartmentIds = filter.DepartmentIds,
            OfficeIds = filter.OfficeIds,
            CommitteeTypeIds = filter.CommitteeTypeIds,
            TermIds = filter.TermIds,
            IsActive = filter.IsActive,
            IsMarketOrientated = filter.IsMarketOrientated,
            HasSupervisionDuty = filter.HasSupervisionDuty
        };
    }

    public static CommitteeUpdateDto ToCommitteeUpdateDto(Committee committee)
    {
        ArgumentNullException.ThrowIfNull(committee);

        return new CommitteeUpdateDto
        {
            Id = committee.Id,
            CommitteeNumber = committee.CommitteeNumber,
            BeginDate = committee.BeginDate,
            EndDate = committee.EndDate,
            IsActive = committee.IsActive,
            DescriptionGerman = committee.DescriptionGerman,
            DescriptionFrench = committee.DescriptionFrench,
            DescriptionItalian = committee.DescriptionItalian,
            DescriptionRomansh = committee.DescriptionRomansh,
            LevelId = committee.CommitteeLevelId,
            OfficeId = committee.OfficeId,
            DepartmentId = committee.DepartmentId,
            CommitteeTypeId = committee.CommitteeTypeId,
            FederalLawEstablishment = committee.FederalLawEstablishment,
            SupervisionDuty = committee.SupervisionDuty,
            MarketOrientated = committee.MarketOrientated,
            LegalFormId = committee.LegalFormId,
            OldLegalForm = committee.OldLegalForm,
            LegalBase = committee.LegalBase,
            TermOfOfficeId = committee.TermOfOfficeId,
            MinimalMembers = committee.MinimalMembers,
            MaximalMembers = committee.MaximalMembers,
            AdditionalAuthorityMembers = committee.AdditionalAuthorityMembers,
            LinkAuthorityWebsite = committee.LinkAuthorityWebsite,
            LinkHomepageGerman = committee.LinkHomepageGerman,
            LinkHomepageFrench = committee.LinkHomepageFrench,
            LinkHomepageItalian = committee.LinkHomepageItalian,
            LinkHomepageRomansh = committee.LinkHomepageRomansh,
            FederalInstitution = committee.FederalInstitution,
            SelfOrganized = committee.SelfOrganized,
            MembersCount = committee.ActiveMemberCount,
            VacanciesInGeneralElection = committee.VacanciesGeneralElection,
            MembershipAdditionsInGeneralElection = committee.MembershipAdditionsInGeneralElection.Select(x => x.Id).ToArray(),
            FutureGeneralElectionCommittee = committee.FutureGeneralElectionCommittee,
            RowVersion = committee.RowVersion
        };
    }

    public static CommitteeExportFilterParametersDto ToCommitteeExportFilterParametersDto(RequestAndReportsFilterParametersDto? filterDto)
    {
        return new CommitteeExportFilterParametersDto
        {
            DepartmentIds = filterDto?.DepartmentIds,
            OfficeIds = filterDto?.OfficeIds,
            CommitteeTypeIds = filterDto?.CommitteeTypeIds,
            ReportType = filterDto?.ReportType,
            AnalysisDate1 = filterDto?.AnalysisDate1,
            AnalysisDate2 = filterDto?.AnalysisDate2,
        };
    }

    public static DimensionItem ToDimensionItem(Committee committee)
    {
        ArgumentNullException.ThrowIfNull(committee);

        var dimensionItem =
            new DimensionItem(
                committee.OgdId,
                new Literal(committee.DescriptionGerman, OgdExportConstants.LanguageDe),
                [
                    new AdditionalLiteralProperty(OgdExportConstants.SchemaName, new Literal(committee.DescriptionGerman, OgdExportConstants.LanguageDe)),
                    new AdditionalLiteralProperty(OgdExportConstants.SchemaName, new Literal(committee.DescriptionFrench, OgdExportConstants.LanguageFr)),
                    new AdditionalLiteralProperty(OgdExportConstants.SchemaName, new Literal(committee.DescriptionItalian, OgdExportConstants.LanguageIt)),
                    new AdditionalLiteralProperty(OgdExportConstants.SchemaName, new Literal(committee.DescriptionRomansh, OgdExportConstants.LanguageRm))
                ]);

        foreach (var contactPoint in committee.ContactPoints)
        {
            if (contactPoint.ContactPointTypeId == ContactPointType.SecretariatGuid)
            {
                //the uri of the secretariat has to match the one exported by OgdExportService
                dimensionItem.AdditionalUriProperties.Add(new AdditionalUriProperty(OgdExportConstants.CommitteeHasSecretariat, $"{OgdExportConstants.NamespaceOrganization}:{contactPoint.OgdId}"));
            }

            if (contactPoint.ContactPointTypeId == ContactPointType.DataProtectionOfficerGuid)
            {
                //the uri of the DPO has to match the one exported by OgdExportService
                dimensionItem.AdditionalUriProperties.Add(new AdditionalUriProperty(OgdExportConstants.CommitteeHasDataProtectionOfficer, $"{OgdExportConstants.NamespaceOrganization}:{contactPoint.OgdId}"));
            }
        }

        if (committee.LatestInstitutionAppointmentDecision is not null)
        {
            dimensionItem.AdditionalUriProperties.Add(new AdditionalUriProperty(OgdExportConstants.SchemaSubjectOf, $"{OgdExportConstants.NamespaceAppointmentDecision}:{committee.LatestInstitutionAppointmentDecision.OgdId}"));
        }

        if (committee.LegalForm is not null)
        {
            dimensionItem.AdditionalUriProperties.Add(new AdditionalUriProperty(OgdExportConstants.CommitteeHasLegalForm, OgdExportConstants.CreateUriLinkForLdAdminCh(committee.LegalForm!.Uri)));
        }

#pragma warning disable CA1308
        dimensionItem.AdditionalLiteralProperties.Add(new AdditionalLiteralProperty(OgdExportConstants.CommitteeAdditionalAuthorityMembers, new Literal(committee.AdditionalAuthorityMembers.ToString(CultureInfo.InvariantCulture).ToLowerInvariant(), new Uri(OgdExportConstants.DataTypeBoolean))));
#pragma warning restore CA1308

        return dimensionItem;
    }

    public static ObservationDataRow ToObservation(Committee committee)
    {
        ArgumentNullException.ThrowIfNull(committee);

        var dataRow = new ObservationDataRow
        {
            KeyUri = $"{OgdExportConstants.NamespaceCommittee}:{committee.OgdId}"
        };

        dataRow.KeyDimensionLinks.AddRange([
            new KeyDimensionLink
            {
                Predicate = $"{OgdExportConstants.NamespaceCommittee}:hasCommittee", Uri = $"{OgdExportConstants.NamespaceCommittee}:{committee.OgdId}", ShapePropertyMetadata = new ShapePropertyMetadata
                {
                    NameDe = "Gremiumname",
                    NameFr = "Nom de l'organe",
                    NameIt = "Nome dell'organo",
                    NameEn = "Committee name",
                    Type = OgdExportConstants.CubeKeyDimension,
                    NodeKind = OgdExportConstants.ShaclNodeKindIri,
                    ScaleType = OgdExportConstants.QudtNominalScale,
                    MinCount = 1,
                    MaxCount = 1
                }
            },
            new KeyDimensionLink
            {
                Predicate = $"{OgdExportConstants.NamespaceCommittee}:hasCommitteeType", Uri = $"{OgdExportConstants.NamespaceCommitteeType}:{committee.CommitteeType!.OgdId}", ShapePropertyMetadata = new ShapePropertyMetadata
                {
                    NameDe = "Gremiumart",
                    NameFr = "Type d'organe",
                    NameIt = "Tipo di organo",
                    NameEn = "Committee type",
                    Type = OgdExportConstants.CubeKeyDimension,
                    NodeKind = OgdExportConstants.ShaclNodeKindIri,
                    ScaleType = OgdExportConstants.QudtNominalScale,
                    MinCount = 1,
                    MaxCount = 1
                }
            },
            new KeyDimensionLink
            {
                Predicate = $"{OgdExportConstants.NamespaceCommittee}:hasDepartment", Uri = OgdExportConstants.CreateUriLinkForLdAdminCh(committee.Department!.Uri), ShapePropertyMetadata = new ShapePropertyMetadata
                {
                    NameDe = "Department",
                    NameFr = "Département",
                    NameIt = "Dipartimento",
                    NameEn = "Department",
                    Type = OgdExportConstants.CubeKeyDimension,
                    NodeKind = OgdExportConstants.ShaclNodeKindIri,
                    ScaleType = OgdExportConstants.QudtNominalScale,
                    MinCount = 1,
                    MaxCount = 1
                }
            }
        ]);

        return dataRow;
    }

    public static CommitteeJustificationUpdateDto ToCommitteeJustificationUpdateDto(Committee committee)
    {
        ArgumentNullException.ThrowIfNull(committee);

        return new CommitteeJustificationUpdateDto
        {
            Id = committee.Id,
            JustificationMembers = committee.JustificationMembers,
            JustificationGenders = committee.JustificationGenders,
            MeasuresGenders = committee.MeasuresGenders,
            JustificationLanguages = committee.JustificationLanguages,
            MeasuresLanguages = committee.MeasuresLanguages,
            CurrentMemberCount = committee.ActiveMemberCount,
            CurrentGenderQuota = GenerateGenderQuotaString(committee),
            CurrentLanguageQuota = GenerateLanguageQuotaString(committee),
            RowVersion = committee.RowVersion
        };
    }

    private static string GenerateGenderQuotaString(Committee committee)
    {
        return $"{committee.MaleQuota} % {BusinessTexts.Common_MaleAbbreviation}, {committee.FemaleQuota} % {BusinessTexts.Common_FemaleAbbreviation}";
    }

    private static string GenerateLanguageQuotaString(Committee committee)
    {
        var usePercentages = committee.CommitteeType!.GermanThresholdPercentage is not null;

        return usePercentages
            ? $"{committee.GermanQuota} % DE, {committee.FrenchQuota} % FR, {committee.ItalianQuota} % IT, {committee.RomanshQuota} % RM"
            : $"{committee.GermanCount} DE, {committee.FrenchCount} FR, {committee.ItalianCount} IT, {committee.RomanshCount} RM";
    }
}
