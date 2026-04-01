using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.CrossCutting;
using Swiss.FCh.Cube.RawData.Model;

namespace Bk.APG.Business.Mapper;

public static class OgdFunctionStatisticMapper
{
    public static ObservationDataRow ToFunctionStatisticObservation(MembershipFunctionStatisticDto statisticDto)
    {
        ArgumentNullException.ThrowIfNull(statisticDto);

        var ogdId = "TOTAL";
        var ogdNamespace = OgdExportConstants.NamespaceCommitteeFunctionStatistic;

        if (statisticDto.FunctionOgdId != 0)
        {
            ogdId = statisticDto.FunctionOgdId.ToString(CultureInfo.InvariantCulture);
        }

        var dataRow = new ObservationDataRow
        {
            // combined key committee and function
            KeyUri = $"{ogdNamespace}:{statisticDto.CommitteeOgdId}-{ogdId}"
        };

        dataRow.KeyDimensionLinks.Add(new KeyDimensionLink
        {
            Predicate = $"{ogdNamespace}:hasCommittee",
            Uri = $"committee:{statisticDto.CommitteeOgdId}",
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "Gremiumname",
                NameFr = "Nom de l'organe",
                NameIt = "Nome dell'organo",
                NameEn = "Committee name",
                Type = OgdExportConstants.CubeKeyDimension,
                NodeKind = OgdExportConstants.ShaclNodeKindIri
            }
        });

        dataRow.KeyDimensionLinks.Add(new KeyDimensionLink
        {
            Predicate = $"{ogdNamespace}:hasFunction",
            Uri = $"function:{ogdId}",
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "Funktion",
                NameFr = "Fonction",
                NameIt = "Funzione",
                NameEn = "Function",
                Type = OgdExportConstants.CubeKeyDimension,
                NodeKind = OgdExportConstants.ShaclNodeKindIri
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:functionCount",
            Object = statisticDto.FunctionCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "# Funktionen",
                NameFr = "# Fonctions",
                NameIt = "# Funzioni",
                NameEn = "# Functions",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        return dataRow;
    }
}
