using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.CrossCutting;
using Swiss.FCh.Cube.Dimension.Model;

namespace Bk.APG.Business.Mapper;

public static class MasterDataMapper
{
    public static T MapToMasterDataDto<T>(MasterDataBase masterData, CultureInfo cultureInfo) where T : MasterDataDtoBase, new()
    {
        return new T
        {
            Id = masterData.Id,
            Text = masterData.GetText(cultureInfo),
            Description = masterData.GetDescription(cultureInfo),
            Uri = masterData.Uri,
            IsDeleted = masterData.IsDeleted,
        };
    }

    public static T MapFunctionToMasterDataDto<T>(Function masterData, CultureInfo cultureInfo) where T : FunctionDto, new()
    {
        return new T
        {
            Id = masterData.Id,
            Text = masterData.GetText(cultureInfo),
            TextFemale = masterData.GetFemaleText(cultureInfo),
            Description = masterData.GetDescription(cultureInfo),
            Uri = masterData.Uri,
            IsDeleted = masterData.IsDeleted,
        };
    }

    public static T MapWorklistTaskTypeToMasterDataDto<T>(WorklistTaskType masterData, CultureInfo cultureInfo) where T : WorklistTaskTypeDto, new()
    {
        return new T
        {
            Id = masterData.Id,
            Text = masterData.GetText(cultureInfo),
            CanBeCreatedManually = masterData.CanBeCreatedManually,
            Description = masterData.GetDescription(cultureInfo),
            Uri = masterData.Uri,
            IsDeleted = masterData.IsDeleted,
        };
    }

    public static T MapTermOfOfficeDateToMasterDataDto<T>(TermOfOfficeDate masterData, CultureInfo cultureInfo) where T : TermDateDto, new()
    {
        return new T
        {
            Id = masterData.Id,
            Text = masterData.GetText(cultureInfo),
            Description = masterData.GetDescription(cultureInfo),
            Uri = masterData.Uri,
            IsDeleted = masterData.IsDeleted,
            BeginDate = masterData.BeginDate,
            EndDate = masterData.EndDate,
            IsGeneralElection = masterData.IsGeneralElection
        };
    }

    public static LegislaturePeriodDto MapToLegislaturePeriodDto(LegislaturePeriod legislaturePeriod)
    {
        return new LegislaturePeriodDto
        {
            ElectionDate = legislaturePeriod.ElectionDate,
            StartDate = legislaturePeriod.StartDate,
            EndDate = legislaturePeriod.EndDate,
            Id = legislaturePeriod.Id,
            Text = legislaturePeriod.GetText()
        };
    }

    public static CouncilDto MapToCouncilDto(Council council)
    {
        return new CouncilDto
        {
            Id = council.Id,
            Text = council.GetText(),
            Description = council.GetText(),
            Sort = council.Sort
        };
    }

    public static DimensionItem ToDimensionItem(MasterDataBase masterDataItem)
    {
        var textAndDescriptions = new List<AdditionalLiteralProperty>();

        if (!string.IsNullOrWhiteSpace(masterDataItem.DescriptionDe))
        {
            textAndDescriptions.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaDescription, new Literal(masterDataItem.DescriptionDe, OgdExportConstants.LanguageDe)));
        }

        if (!string.IsNullOrWhiteSpace(masterDataItem.DescriptionFr))
        {
            textAndDescriptions.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaDescription, new Literal(masterDataItem.DescriptionFr, OgdExportConstants.LanguageFr)));
        }

        if (!string.IsNullOrWhiteSpace(masterDataItem.DescriptionIt))
        {
            textAndDescriptions.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaDescription, new Literal(masterDataItem.DescriptionIt, OgdExportConstants.LanguageIt)));
        }

        if (!string.IsNullOrWhiteSpace(masterDataItem.DescriptionRm))
        {
            textAndDescriptions.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaDescription, new Literal(masterDataItem.DescriptionRm, OgdExportConstants.LanguageRm)));
        }

        if (!string.IsNullOrWhiteSpace(masterDataItem.TextFr))
        {
            textAndDescriptions.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaName, new Literal(masterDataItem.TextFr, OgdExportConstants.LanguageFr)));
        }

        if (!string.IsNullOrWhiteSpace(masterDataItem.TextIt))
        {
            textAndDescriptions.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaName, new Literal(masterDataItem.TextIt, OgdExportConstants.LanguageIt)));
        }

        if (!string.IsNullOrWhiteSpace(masterDataItem.TextRm))
        {
            textAndDescriptions.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaName, new Literal(masterDataItem.TextRm, OgdExportConstants.LanguageRm)));
        }

        textAndDescriptions.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaPosition, new Literal(masterDataItem.Sort.ToString(), new Uri(OgdExportConstants.DataTypeInt))));

        var dimensionItem =
            new DimensionItem(
                masterDataItem.OgdId,
                new Literal(masterDataItem.TextDe, OgdExportConstants.LanguageDe),
                textAndDescriptions);

        return dimensionItem;
    }

    public static DepartmentDto MapToDepartmentDto(Department department)
    {
        return new DepartmentDto
        {
            Id = department.Id,
            Text = department.GetText(),
            Description = department.GetDescription(),
            Uri = department.Uri,
            IsDeleted = department.IsDeleted,
        };
    }

    public static OfficeDto MapToOfficeDto(Office office, CultureInfo cultureInfo)
    {
        return new OfficeDto
        {
            Id = office.Id,
            Text = office.GetText(cultureInfo),
            DepartmentId = office.DepartmentId,
            Description = office.GetDescription(cultureInfo),
            Uri = office.Uri,
            IsDeleted = office.IsDeleted,
        };
    }

    public static OccupationDto MapToOccupationDto(Occupation occupation, CultureInfo cultureInfo)
    {
        return new OccupationDto
        {
            Id = occupation.Id,
            Text = occupation.GetText(cultureInfo),
            TextFemale = occupation.GetFemaleText(cultureInfo),
            Description = occupation.GetDescription(cultureInfo),
            Uri = occupation.Uri,
            IsDeleted = occupation.IsDeleted,
        };
    }
}
