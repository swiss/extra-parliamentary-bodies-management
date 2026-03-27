using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.CrossCutting;
using Swiss.FCh.Cube.RawData.Model;

namespace Bk.APG.Business.Mapper;

public static class OgdCantonStatisticMapper
{
    public static ObservationDataRow ToCantonStatisticObservation(MembershipCantonStatisticDto statisticDto)
    {
        ArgumentNullException.ThrowIfNull(statisticDto);

        var ogdNamespace = OgdExportConstants.NamespaceCommitteeCantonStatistic;

        var dataRow = new ObservationDataRow
        {
            // combined key committee and canton
            KeyUri = $"{ogdNamespace}:{statisticDto.CommitteeOgdId}-{statisticDto.CantonOgdId}"
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

        // According to Michael Luggen, the "Ausland" records should be treated like this, as there is no official record in LINDAS. Might change in the future.
        dataRow.KeyDimensionLinks.Add(new KeyDimensionLink
        {
            Predicate = $"{ogdNamespace}:hasCanton",
            Uri = statisticDto.CantonUri == "www.todo.uri.Ausland" ? $"canton:{statisticDto.CantonOgdId}" : OgdExportConstants.CreateUriLinkForLdAdminCh(statisticDto.CantonUri),
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "Kanton",
                NameFr = "Canton",
                NameIt = "Cantone",
                NameEn = "Canton",
                Type = OgdExportConstants.CubeKeyDimension,
                NodeKind = OgdExportConstants.ShaclNodeKindIri
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:cantonCount",
            Object = statisticDto.CantonCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "# Kantone",
                NameFr = "# Cantons",
                NameIt = "# Cantoni",
                NameEn = "# Cantons",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });
        return dataRow;
    }
}
