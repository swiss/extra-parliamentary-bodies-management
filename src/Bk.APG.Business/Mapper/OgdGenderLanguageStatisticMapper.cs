using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.CrossCutting;
using Swiss.FCh.Cube.RawData.Model;

namespace Bk.APG.Business.Mapper;

public static class OgdGenderLanguageStatisticMapper
{
    public static ObservationDataRow ToGenderLanguageStatisticObservation(MembershipGenderLanguageStatisticDto statisticDto)
    {
        var ogdNamespace = OgdExportConstants.NamespaceCommitteeGenderLanguageStatistic;
        var committeeType = OgdExportConstants.NamespaceCommitteeType;

        var dataRow = new ObservationDataRow();

        // has 3 different usages, usage 1 for committee statistic
        if (statisticDto.CommitteeOgdId != null)
        {
            dataRow.KeyUri = $"{ogdNamespace}:{statisticDto.CommitteeOgdId}";

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
        }
        // usage 2, for committeeType statistic
        else if (statisticDto.CommitteeTypeOgdId != null && statisticDto.Department == null)
        {
            dataRow.KeyUri = $"{ogdNamespace}:{committeeType}-{statisticDto.CommitteeTypeOgdId}";
        }
        // usage 3, for calculated committeeTypes (APK/NON-APK) by department using self generated committeeOgdId
        else
        {
            dataRow.KeyUri = $"{ogdNamespace}:{statisticDto.CommitteeTypeOgdId}-{statisticDto.Department}";
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

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:totalCount",
            Object = statisticDto.Count.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "# Total",
                NameFr = "# Total",
                NameIt = "# Totale",
                NameEn = "# Total",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:femaleCount",
            Object = statisticDto.FemaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "# Frauen",
                NameFr = "# Femmes",
                NameIt = "# Donne",
                NameEn = "# Women",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:femalePercentage",
            Object = statisticDto.FemalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "% Frauen",
                NameFr = "% Femmes",
                NameIt = "% Donne",
                NameEn = "% Women",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:maleCount",
            Object = statisticDto.MaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "# Männer",
                NameFr = "# Hommes",
                NameIt = "# Uomini",
                NameEn = "# Men",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:malePercentage",
            Object = statisticDto.MalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "% Männer",
                NameFr = "% Hommes",
                NameIt = "% Uomini",
                NameEn = "% Men",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:germanCount",
            Object = statisticDto.GermanCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "# Deutsch",
                NameFr = "# Allemand",
                NameIt = "# Tedesco",
                NameEn = "# German",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:germanPercentage",
            Object = statisticDto.GermanPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "% Deutsch",
                NameFr = "% Allemand",
                NameIt = "% Tedesco",
                NameEn = "% German",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:frenchCount",
            Object = statisticDto.FrenchCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "# Französisch",
                NameFr = "# Français",
                NameIt = "# Francese",
                NameEn = "# French",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:frenchPercentage",
            Object = statisticDto.FrenchPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "% Französisch",
                NameFr = "% Français",
                NameIt = "% Francese",
                NameEn = "% French",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:italianCount",
            Object = statisticDto.ItalianCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "# Italienisch",
                NameFr = "# Italien",
                NameIt = "# Italiano",
                NameEn = "# Italian",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:italianPercentage",
            Object = statisticDto.ItalianPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "% Italienisch",
                NameFr = "% Italien",
                NameIt = "% Italiano",
                NameEn = "% Italian",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:romanshCount",
            Object = statisticDto.RomanshCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "# Rätoromanisch",
                NameFr = "# Romanche",
                NameIt = "# Romancio",
                NameEn = "# Romansh",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:romanshPercentage",
            Object = statisticDto.RomanshPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "% Rätoromanisch",
                NameFr = "% Romanche",
                NameIt = "% Romancio",
                NameEn = "% Romansh",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:federalDutyCount",
            Object = statisticDto.FederalDutyCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "# Bundesdienst",
                NameFr = "# Service de la Confédération",
                NameIt = "# Servizio della Confederazione",
                NameEn = "# Federal duty",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:federalAssemblyCount",
            Object = statisticDto.FederalAssemblyCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "# Bundesversammlung",
                NameFr = "# Assemblée fédérale",
                NameIt = "# Assemblea federale",
                NameEn = "# Federal assembly",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:upTo30Count",
            Object = statisticDto.UpTo30Count.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "# Bis 30",
                NameFr = "# Jusqu'à 30",
                NameIt = "# Fino a 30",
                NameEn = "# Up to 30",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:upTo30Percentage",
            Object = statisticDto.UpTo30Percentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "% Bis 30",
                NameFr = "% Jusqu'à 30",
                NameIt = "% Fino a 30",
                NameEn = "% Up to 30",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:from31to40Count",
            Object = statisticDto.From31To40Count.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "# 31 bis 40",
                NameFr = "# 31 à 40",
                NameIt = "# 31 a 40",
                NameEn = "# 31 to 40",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:from31To40Percentage",
            Object = statisticDto.From31To40Percentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "% 31 bis 40",
                NameFr = "% 31 à 40",
                NameIt = "% 31 a 40",
                NameEn = "% 31 to 40",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:from41to50Count",
            Object = statisticDto.From41To50Count.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "# 41 bis 50",
                NameFr = "# 41 à 50",
                NameIt = "# 41 a 50",
                NameEn = "# 41 to 50",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:from41To50Percentage",
            Object = statisticDto.From41To50Percentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "% 41 bis 50",
                NameFr = "% 41 à 50",
                NameIt = "% 41 a 50",
                NameEn = "% 41 to 50",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:from51to60Count",
            Object = statisticDto.From51To60Count.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "# 51 bis 60",
                NameFr = "# 51 à 60",
                NameIt = "# 51 a 60",
                NameEn = "# 51 to 60",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:from51To60Percentage",
            Object = statisticDto.From51To60Percentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "% 51 bis 60",
                NameFr = "% 51 à 60",
                NameIt = "% 51 a 60",
                NameEn = "% 51 to 60",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:from61to70Count",
            Object = statisticDto.From61To70Count.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "# 61 bis 70",
                NameFr = "# 61 à 70",
                NameIt = "# 61 a 70",
                NameEn = "# 61 to 70",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:from61To70Percentage",
            Object = statisticDto.From61To70Percentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "% 61 bis 70",
                NameFr = "% 61 à 70",
                NameIt = "% 61 a 70",
                NameEn = "% 61 to 70",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:over70Count",
            Object = statisticDto.Over70Count.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "# Über 70",
                NameFr = "# Plus de 70",
                NameIt = "# Oltre 70",
                NameEn = "# Over 70",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:over70Percentage",
            Object = statisticDto.Over70Percentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "% Über 70",
                NameFr = "% Plus de 70",
                NameIt = "% Oltre 70",
                NameEn = "% Over 70",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        return dataRow;
    }
}
