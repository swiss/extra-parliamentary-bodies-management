using System.Globalization;
using Bk.APG.Business.Models;
using Bk.APG.CrossCutting;
using Swiss.FCh.Cube.Dimension.Model;

namespace Bk.APG.Business.Mapper;

public static class FunctionMapper
{
    public static DimensionItem ToDimensionItem(Function function)
    {
        ArgumentNullException.ThrowIfNull(function);

        var functions = new List<AdditionalLiteralProperty>();

        if (!string.IsNullOrWhiteSpace(function.TextFr))
        {
            functions.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaName, new Literal(function.TextFr, OgdExportConstants.LanguageFr)));
        }

        if (!string.IsNullOrWhiteSpace(function.TextIt))
        {
            functions.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaName, new Literal(function.TextIt, OgdExportConstants.LanguageIt)));
        }

        if (!string.IsNullOrWhiteSpace(function.TextRm))
        {
            functions.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaName, new Literal(function.TextRm, OgdExportConstants.LanguageRm)));
        }

        if (!string.IsNullOrWhiteSpace(function.TextFemaleDe))
        {
            functions.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaName, new Literal(function.TextFemaleDe, OgdExportConstants.LanguageDe)));
        }

        if (!string.IsNullOrWhiteSpace(function.TextFemaleFr))
        {
            functions.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaName, new Literal(function.TextFemaleFr, OgdExportConstants.LanguageFr)));
        }

        if (!string.IsNullOrWhiteSpace(function.TextFemaleIt))
        {
            functions.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaName, new Literal(function.TextFemaleIt, OgdExportConstants.LanguageIt)));
        }

        if (!string.IsNullOrWhiteSpace(function.TextFemaleRm))
        {
            functions.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaName, new Literal(function.TextFemaleRm, OgdExportConstants.LanguageRm)));
        }

        functions.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaPosition, new Literal(function.Sort.ToString(CultureInfo.InvariantCulture), new Uri(OgdExportConstants.DataTypeInt))));

        var dimensionItem =
            new DimensionItem(
                function.OgdId,
                new Literal(function.TextDe, OgdExportConstants.LanguageDe),
                functions);

        return dimensionItem;
    }
}
