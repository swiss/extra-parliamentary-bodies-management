using Bk.APG.Business.Dtos;
using Bk.APG.Business.Extensions;
using Bk.APG.Business.Models;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting;
using Swiss.FCh.Cube.RawData.Model;

namespace Bk.APG.Business.Mapper;

public static class MembershipMapper
{
    public static MembershipDetailDto ToMembershipDetailDto(Membership membership)
    {
        return new MembershipDetailDto
        {
            Id = membership.Id,
            Function = membership.Function!.GetText(),
            Committee = membership.Committee!.GetDescription(),
            BeginDate = membership.BeginDate,
            EndDate = membership.EndDate,
            IsActive = membership.IsActive,
            NeedsAttention = membership.NeedsAttention
        };
    }

    public static PersonMembershipDto ToPersonMembershipDto(Membership membership)
    {
        return new PersonMembershipDto
        {
            Id = membership.Id,
            Committee = membership.Committee!.GetDescription(),
            Department = membership.Committee!.Department!.GetText(),
            Function = membership.Function!.GetText(),
            BeginDate = membership.BeginDate,
            EndDate = membership.EndDate,
            ElectionType = membership.ElectionType!.GetText(),
            IsActive = membership.IsActive,
            NeedsAttention = membership.NeedsAttention
        };
    }

    public static CommitteeMemberDto ToCommitteeMemberDto(Membership membership)
    {
        return new CommitteeMemberDto
        {
            Id = membership.Id,
            PersonId = membership.PersonId,
            Surname = membership.Person!.Surname,
            GivenName = membership.Person!.GivenName,
            Gender = membership.Person!.Gender!.GetText(),
            Language = membership.Person!.Language!.GetText(),
            EmploymentLevel = membership.GetEmploymentLevel(),
            Function = membership.Person!.GenderId == Gender.FemaleGuid ? membership.Function!.GetFemaleText() : membership.Function!.GetText(),
            BeginDate = membership.BeginDate,
            EndDate = membership.EndDate,
            ElectionType = membership.ElectionType!.GetText(),
            HasMembershipAddition = membership.MembershipAddition is not null,
            IsActive = membership.IsActive,
            IsFuture = membership.IsFuture,
            NeedsAttention = membership.NeedsAttention || membership.NeedsAttentionInterests
        };
    }

    public static Membership FromMembershipCreateDto(MembershipCreateDto membershipCreateDto, string currentUserName)
    {
        return new Membership
        {
            CommitteeId = membershipCreateDto.CommitteeId,
            PersonId = membershipCreateDto.PersonId,
            MaximumEmploymentLevel = membershipCreateDto.MaximumEmploymentLevel,
            BeginDate = membershipCreateDto.BeginDate,
            EndDate = membershipCreateDto.EndDate,
            ElectionTypeId = ElectionType.NewElectionGuid,
            FunctionId = membershipCreateDto.FunctionId,
            ElectionOfficeId = membershipCreateDto.ElectionOfficeId,
            OldMembershipAddition = null,
            MembershipAdditionId = membershipCreateDto.MembershipAdditionId,
            JustificationLongerDuty = membershipCreateDto.JustificationLongerDuty,
            JustificationShorterDuty = membershipCreateDto.JustificationShorterDuty,
            JustificationMemberInFederalDuty = membershipCreateDto.JustificationMemberInFederalDuty,
            JustificationMemberInFederalAssembly = membershipCreateDto.JustificationMemberInFederalAssembly,
            RequirementsProfile = membershipCreateDto.RequirementsProfile,
            Remarks = membershipCreateDto.Remarks,
            RemarksStatus = membershipCreateDto.RemarksStatus,
            Created = DateTime.UtcNow,
            CreatedBy = currentUserName,
            Modified = DateTime.UtcNow,
            ModifiedBy = currentUserName,
            InCorrelationWithFederalDuty = membershipCreateDto.InCorrelationWithFederalDuty,
            IsDeleted = false
        };
    }

    public static MembershipUpdateDto ToMembershipUpdateDto(Membership membership, ICultureService cultureService)
    {
        return new MembershipUpdateDto
        {
            Id = membership.Id,
            PersonId = membership.PersonId,
            CommitteeId = membership.CommitteeId,
            MaximumEmploymentLevel = membership.MaximumEmploymentLevel,
            BeginDate = membership.BeginDate,
            EndDate = membership.EndDate,
            ElectionTypeId = membership.ElectionTypeId,
            FunctionId = membership.FunctionId,
            FunctionName = membership.FunctionName,
            ElectionOfficeId = membership.ElectionOfficeId,
            ElectionOfficeName = membership.ElectionOffice is not null ? membership.ElectionOffice.GetText() : string.Empty,
            OldMembershipAddition = membership.OldMembershipAddition,
            MembershipAdditionId = membership.MembershipAdditionId,
            JustificationLongerDuty = membership.JustificationLongerDuty,
            JustificationShorterDuty = membership.JustificationShorterDuty,
            JustificationMemberInFederalDuty = membership.JustificationMemberInFederalDuty,
            JustificationMemberInFederalAssembly = membership.JustificationMemberInFederalAssembly,
            RequirementsProfile = membership.RequirementsProfile,
            Remarks = membership.Remarks,
            RemarksStatus = membership.RemarksStatus,
            InCorrelationWithFederalDuty = membership.InCorrelationWithFederalDuty,
            RowVersion = membership.RowVersion,
            MembershipAddition = membership.MembershipAddition is not null ? MasterDataMapper.MapToMasterDataDto<MembershipAdditionDto>(membership.MembershipAddition, cultureService.GetCurrentUiCulture()) : null
        };
    }

    public static ObservationDataRow ToObservation(Membership membership)
    {
        ArgumentNullException.ThrowIfNull(membership.Committee, nameof(membership.Committee));
        ArgumentNullException.ThrowIfNull(membership.Person, nameof(membership.Person));

        var dataRow = new ObservationDataRow
        {
            KeyUri = $"{OgdExportConstants.NamespaceMembership}:{membership.OgdId}",
            ValidFrom = membership.BeginDate.ToDateTime(TimeOnly.MinValue),
            ValidTo = membership.EndDate.ToDateTime(TimeOnly.MinValue),
        };

        dataRow.KeyDimensionLinks.Add(
            new KeyDimensionLink { Predicate = $"{OgdExportConstants.NamespaceMembership}:hasPerson", Uri = $"person:{membership.Person.OgdId}" });

        dataRow.KeyDimensionLinks.Add(
            new KeyDimensionLink { Predicate = $"{OgdExportConstants.NamespaceMembership}:hasCommittee", Uri = $"committee:{membership.Committee.OgdId}" });

        if (membership.Committee.CommitteeType is not null)
        {
            dataRow.KeyDimensionLinks.Add(
                new KeyDimensionLink { Predicate = $"{OgdExportConstants.NamespaceMembership}:hasCommitteeType", Uri = $"committee-type:{membership.Committee.CommitteeType.OgdId}" });
        }

        if (membership.ElectionOffice is not null)
        {
            dataRow.KeyDimensionLinks.Add(
                new KeyDimensionLink { Predicate = $"{OgdExportConstants.NamespaceMembership}:voteBody", Uri = OgdExportConstants.CreateUriLinkForLdAdminCh(membership.ElectionOffice.Uri) });
        }

        if (membership.Committee.Department is not null)
        {
            dataRow.KeyDimensionLinks.Add(
                new KeyDimensionLink { Predicate = $"{OgdExportConstants.NamespaceMembership}:hasDepartment", Uri = OgdExportConstants.CreateUriLinkForLdAdminCh(membership.Committee.Department.Uri) });
        }

        if (membership.Function is not null)
        {
            dataRow.KeyDimensionLinks.Add(
                new KeyDimensionLink { Predicate = $"{OgdExportConstants.NamespaceMembership}:hasFunction", Uri = $"function:{membership.Function.OgdId}" });
        }

        if (membership.MembershipAddition is not null)
        {
            if (!string.IsNullOrWhiteSpace(membership.MembershipAddition.TextDe))
            {
                dataRow.Values.Add(
                    new DimensionValue { Predicate = OgdExportConstants.SchemaDescription, Object = $"{membership.MembershipAddition.TextDe}: {membership.MembershipAddition.DescriptionDe}", LanguageTag = OgdExportConstants.LanguageDe });
            }

            if (!string.IsNullOrWhiteSpace(membership.MembershipAddition.TextFr))
            {
                dataRow.Values.Add(
                    new DimensionValue { Predicate = OgdExportConstants.SchemaDescription, Object = $"{membership.MembershipAddition.TextFr}: {membership.MembershipAddition.DescriptionFr}", LanguageTag = OgdExportConstants.LanguageFr });
            }

            if (!string.IsNullOrWhiteSpace(membership.MembershipAddition.TextIt))
            {
                dataRow.Values.Add(
                    new DimensionValue { Predicate = OgdExportConstants.SchemaDescription, Object = $"{membership.MembershipAddition.TextIt}: {membership.MembershipAddition.DescriptionIt}", LanguageTag = OgdExportConstants.LanguageIt });
            }

            if (!string.IsNullOrWhiteSpace(membership.MembershipAddition.TextRm))
            {
                dataRow.Values.Add(
                    new DimensionValue { Predicate = OgdExportConstants.SchemaDescription, Object = $"{membership.MembershipAddition.TextRm}: {membership.MembershipAddition.DescriptionRm}", LanguageTag = OgdExportConstants.LanguageRm });
            }
        }

        //the following properties would actually belong to the person itself (they should be exported as part of the person dimension)
        //however, we decided to add these to the membership cube, because it makes it easier to query and display the information
        if (membership.Person.Gender is not null && !string.IsNullOrWhiteSpace(membership.Person.Gender.Uri))
        {
            var genderUri = membership.Person.Gender.Uri.Replace("https://register.ld.admin.ch/", "rld:");
            dataRow.KeyDimensionLinks.Add(new KeyDimensionLink { Predicate = OgdExportConstants.SchemaGender, Uri = genderUri });
        }

        if (membership.Person.CorrespondenceAddress?.Canton is not null)
        {
            if (!string.IsNullOrWhiteSpace(membership.Person.CorrespondenceAddress.Canton.Uri) && membership.Person.CorrespondenceAddress.Canton.Uri.Contains("ld.admin.ch"))
            {
                var cantonUri = membership.Person.CorrespondenceAddress.Canton.Uri.Replace("https://ld.admin.ch/", "ld:");
                dataRow.KeyDimensionLinks.Add(new KeyDimensionLink { Predicate = "ld:hasCanton", Uri = cantonUri });
            }
        }

        if (membership.Person.Language is not null)
        {
            dataRow.Values.Add(new DimensionValue { Predicate = OgdExportConstants.SchemaLanguage, Object = membership.Person.Language.Uri });
        }

        return dataRow;
    }
}
