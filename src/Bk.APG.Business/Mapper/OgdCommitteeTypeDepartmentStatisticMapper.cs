using Bk.APG.Business.Dtos;
using Bk.APG.CrossCutting;
using Swiss.FCh.Cube.RawData.Model;

namespace Bk.APG.Business.Mapper;

public static class OgdCommitteeTypeDepartmentStatisticMapper
{
    public static ObservationDataRow ToCommitteeTypeDepartmentStatisticObservation(CommitteeTypeDepartmentStatisticDto statisticDto)
    {
        var ogdNamespace = OgdExportConstants.NamespaceCommitteeTypeDepartmentStatistic;

        var dataRow = new ObservationDataRow
        {
            KeyUri = $"{ogdNamespace}:{statisticDto.OgdId}"
        };

        if (statisticDto.CommitteeTypeOgdId != null)
        {
            dataRow.KeyDimensionLinks.Add(new KeyDimensionLink
            {
                Predicate = $"{ogdNamespace}:hasCommitteeType",
                Uri = $"{OgdExportConstants.NamespaceCommitteeType}:{statisticDto.CommitteeTypeOgdId}",
                ShapePropertyMetadata = new ShapePropertyMetadata
                {
                    NameDe = "Gremienart",
                    NameFr = "Type d'organe",
                    NameIt = "Tipo di organo",
                    NameEn = "Committee type",
                    Type = OgdExportConstants.CubeKeyDimension,
                    NodeKind = OgdExportConstants.ShaclNodeKindIri
                }
            });
        }

        if (statisticDto.DepartmentUri != null)
        {
            dataRow.KeyDimensionLinks.Add(new KeyDimensionLink
            {
                Predicate = $"{ogdNamespace}:hasDepartment",
                Uri = OgdExportConstants.CreateUriLinkForLdAdminCh(statisticDto.DepartmentUri),
                ShapePropertyMetadata = new ShapePropertyMetadata
                {
                    NameDe = "Departement",
                    NameFr = "Département",
                    NameIt = "Dipartimento",
                    NameEn = "Department",
                    Type = OgdExportConstants.CubeKeyDimension,
                    NodeKind = OgdExportConstants.ShaclNodeKindIri
                }
            });
        }

        // TODO PP: Totale als Bund/Federal exportieren? Check!
        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:organisation",
            Object = statisticDto.Organisation,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "Organisation",
                NameFr = "Organisation",
                NameIt = "Organizzazione",
                NameEn = "Organisation",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:committeeCount",
            Object = statisticDto.CommitteeCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "# Gremien",
                NameFr = "# Organes",
                NameIt = "# Organi",
                NameEn = "# Committees",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        return dataRow;
    }
}
