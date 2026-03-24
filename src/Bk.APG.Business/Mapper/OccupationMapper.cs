using Bk.APG.Business.Models;
using Bk.APG.CrossCutting;
using Swiss.FCh.Cube.Dimension.Model;

namespace Bk.APG.Business.Mapper;

public static class OccupationMapper
{
    public static DimensionItem ToDimensionItem(Occupation occupation)
    {
        var occupations = new List<AdditionalLiteralProperty>();

        if (!string.IsNullOrWhiteSpace(occupation.TextFr))
        {
            occupations.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaName, new Literal(occupation.TextFr, OgdExportConstants.LanguageFr)));
        }

        if (!string.IsNullOrWhiteSpace(occupation.TextIt))
        {
            occupations.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaName, new Literal(occupation.TextIt, OgdExportConstants.LanguageIt)));
        }

        if (!string.IsNullOrWhiteSpace(occupation.TextRm))
        {
            occupations.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaName, new Literal(occupation.TextRm, OgdExportConstants.LanguageRm)));
        }

        if (!string.IsNullOrWhiteSpace(occupation.TextFemaleDe))
        {
            occupations.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaName, new Literal(occupation.TextFemaleDe, OgdExportConstants.LanguageDe)));
        }

        if (!string.IsNullOrWhiteSpace(occupation.TextFemaleFr))
        {
            occupations.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaName, new Literal(occupation.TextFemaleFr, OgdExportConstants.LanguageFr)));
        }

        if (!string.IsNullOrWhiteSpace(occupation.TextFemaleIt))
        {
            occupations.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaName, new Literal(occupation.TextFemaleIt, OgdExportConstants.LanguageIt)));
        }

        if (!string.IsNullOrWhiteSpace(occupation.TextFemaleRm))
        {
            occupations.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaName, new Literal(occupation.TextFemaleRm, OgdExportConstants.LanguageRm)));
        }

        occupations.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaPosition, new Literal(occupation.Sort.ToString(), new Uri(OgdExportConstants.DataTypeInt))));

        var dimensionItem =
            new DimensionItem(
                occupation.OgdId,
                new Literal(occupation.TextDe, OgdExportConstants.LanguageDe),
                occupations);

        return dimensionItem;
    }
}
