using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.CrossCutting;
using Swiss.FCh.Cube.RawData.Model;

namespace Bk.APG.Business.Mapper;

public static class OgdCommitteeTypeStatisticMapper
{
    public static ObservationDataRow ToCommitteeTypeStatisticObservation(CommitteeTypeStatisticDto statisticDto)
    {
        ArgumentNullException.ThrowIfNull(statisticDto);

        var ogdNamespace = OgdExportConstants.NamespaceCommitteeTypeStatistic;
        var apkNamespace = OgdExportConstants.NamespaceExtraparliamentaryCommission;
        var nonApkNamespace = OgdExportConstants.NamespaceNonExtraparliamentaryCommission;
        var authoritiesCommissionNamespace = OgdExportConstants.NamespaceAuthoritiesCommission;
        var administrationCommissionNamespace = OgdExportConstants.NamespaceAdministrationCommission;
        var federalAgenciesCommissionNamespace = OgdExportConstants.NamespaceFederalAgenciesCommission;
        var managementCommissionNamespace = OgdExportConstants.NamespaceManagementCommission;

        var departmentEda = OgdExportConstants.DepartmentEda;
        var departmentEdi = OgdExportConstants.DepartmentEdi;
        var departmentEjpd = OgdExportConstants.DepartmentEjpd;
        var departmentVbs = OgdExportConstants.DepartmentVbs;
        var departmentEfd = OgdExportConstants.DepartmentEfd;
        var departmentWbf = OgdExportConstants.DepartmentWbf;
        var departmentUvek = OgdExportConstants.DepartmentUvek;

        var dataRow = new ObservationDataRow
        {
            KeyUri = $"{ogdNamespace}:{1}"
        };

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-count",
            Object = statisticDto.ExtraParliamentaryCommissionsCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "# Ausserp Kommissionen",
                NameFr = "# Commissions extraparlementaires",
                NameIt = "# Commissioni extraparlamentari",
                NameEn = "# Extraparliamentary Commissions",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-count",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentEda}",
                NameFr = $"# {departmentEda}",
                NameIt = $"# {departmentEda}",
                NameEn = $"# {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-count",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentEdi}",
                NameFr = $"# {departmentEdi}",
                NameIt = $"# {departmentEdi}",
                NameEn = $"# {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-count",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentEjpd}",
                NameFr = $"# {departmentEjpd}",
                NameIt = $"# {departmentEjpd}",
                NameEn = $"# {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-count",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentVbs}",
                NameFr = $"# {departmentVbs}",
                NameIt = $"# {departmentVbs}",
                NameEn = $"# {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-count",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentEfd}",
                NameFr = $"# {departmentEfd}",
                NameIt = $"# {departmentEfd}",
                NameEn = $"# {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-count",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentWbf}",
                NameFr = $"# {departmentWbf}",
                NameIt = $"# {departmentWbf}",
                NameEn = $"# {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-count",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentUvek}",
                NameFr = $"# {departmentUvek}",
                NameIt = $"# {departmentUvek}",
                NameEn = $"# {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-count",
            Object = statisticDto.AuthoritiesCommissionsCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "# Behördenkommissionen",
                NameFr = "# Commissions des autorités",
                NameIt = "# Commissioni delle autorità",
                NameEn = "# Authorities Commissions",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-count",
            Object = statisticDto.AdministrationCommissionsCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "# Verwaltungskommissionen",
                NameFr = "# Commissions d'administration",
                NameIt = "# Commissioni di amministrazione",
                NameEn = "# Administration Commissions",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-{departmentEda}-count",
            Object = statisticDto.AuthoritiesCommissionsEdaCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentEda}",
                NameFr = $"# {departmentEda}",
                NameIt = $"# {departmentEda}",
                NameEn = $"# {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-{departmentEdi}-count",
            Object = statisticDto.AuthoritiesCommissionsEdiCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentEdi}",
                NameFr = $"# {departmentEdi}",
                NameIt = $"# {departmentEdi}",
                NameEn = $"# {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-{departmentEjpd}-count",
            Object = statisticDto.AuthoritiesCommissionsEjpdCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentEjpd}",
                NameFr = $"# {departmentEjpd}",
                NameIt = $"# {departmentEjpd}",
                NameEn = $"# {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-{departmentVbs}-count",
            Object = statisticDto.AuthoritiesCommissionsVbsCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentVbs}",
                NameFr = $"# {departmentVbs}",
                NameIt = $"# {departmentVbs}",
                NameEn = $"# {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-{departmentEfd}-count",
            Object = statisticDto.AuthoritiesCommissionsEfdCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentEfd}",
                NameFr = $"# {departmentEfd}",
                NameIt = $"# {departmentEfd}",
                NameEn = $"# {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-{departmentWbf}-count",
            Object = statisticDto.AuthoritiesCommissionsWbfCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentWbf}",
                NameFr = $"# {departmentWbf}",
                NameIt = $"# {departmentWbf}",
                NameEn = $"# {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-{departmentUvek}-count",
            Object = statisticDto.AuthoritiesCommissionsUvekCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentUvek}",
                NameFr = $"# {departmentUvek}",
                NameIt = $"# {departmentUvek}",
                NameEn = $"# {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-{departmentEda}-count",
            Object = statisticDto.AdministrationCommissionsEdaCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentEda}",
                NameFr = $"# {departmentEda}",
                NameIt = $"# {departmentEda}",
                NameEn = $"# {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-{departmentEdi}-count",
            Object = statisticDto.AdministrationCommissionsEdiCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentEdi}",
                NameFr = $"# {departmentEdi}",
                NameIt = $"# {departmentEdi}",
                NameEn = $"# {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-{departmentEjpd}-count",
            Object = statisticDto.AdministrationCommissionsEjpdCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentEjpd}",
                NameFr = $"# {departmentEjpd}",
                NameIt = $"# {departmentEjpd}",
                NameEn = $"# {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-{departmentVbs}-count",
            Object = statisticDto.AdministrationCommissionsVbsCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentVbs}",
                NameFr = $"# {departmentVbs}",
                NameIt = $"# {departmentVbs}",
                NameEn = $"# {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-{departmentEfd}-count",
            Object = statisticDto.AdministrationCommissionsEfdCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentEfd}",
                NameFr = $"# {departmentEfd}",
                NameIt = $"# {departmentEfd}",
                NameEn = $"# {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-{departmentWbf}-count",
            Object = statisticDto.AdministrationCommissionsWbfCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentWbf}",
                NameFr = $"# {departmentWbf}",
                NameIt = $"# {departmentWbf}",
                NameEn = $"# {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-{departmentUvek}-count",
            Object = statisticDto.AdministrationCommissionsUvekCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentUvek}",
                NameFr = $"# {departmentUvek}",
                NameIt = $"# {departmentUvek}",
                NameEn = $"# {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-femaleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaFemaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Frauen {departmentEda}",
                NameFr = $"# Femmes {departmentEda}",
                NameIt = $"# Donne {departmentEda}",
                NameEn = $"# Women {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-femaleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiFemaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Frauen {departmentEdi}",
                NameFr = $"# Femmes {departmentEdi}",
                NameIt = $"# Donne {departmentEdi}",
                NameEn = $"# Women {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-femaleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdFemaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Frauen {departmentEjpd}",
                NameFr = $"# Femmes {departmentEjpd}",
                NameIt = $"# Donne {departmentEjpd}",
                NameEn = $"# Women {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-femaleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsFemaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Frauen {departmentVbs}",
                NameFr = $"# Femmes {departmentVbs}",
                NameIt = $"# Donne {departmentVbs}",
                NameEn = $"# Women {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-femaleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdFemaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Frauen {departmentEfd}",
                NameFr = $"# Femmes {departmentEfd}",
                NameIt = $"# Donne {departmentEfd}",
                NameEn = $"# Women {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-femaleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfFemaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Frauen {departmentWbf}",
                NameFr = $"# Femmes {departmentWbf}",
                NameIt = $"# Donne {departmentWbf}",
                NameEn = $"# Women {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-femaleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekFemaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Frauen {departmentUvek}",
                NameFr = $"# Femmes {departmentUvek}",
                NameIt = $"# Donne {departmentUvek}",
                NameEn = $"# Women {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-maleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaMaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Männer {departmentEda}",
                NameFr = $"# Hommes {departmentEda}",
                NameIt = $"# Uomini {departmentEda}",
                NameEn = $"# Men {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-maleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiMaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Männer {departmentEdi}",
                NameFr = $"# Hommes {departmentEdi}",
                NameIt = $"# Uomini {departmentEdi}",
                NameEn = $"# Men {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-maleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdMaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Männer {departmentEjpd}",
                NameFr = $"# Hommes {departmentEjpd}",
                NameIt = $"# Uomini {departmentEjpd}",
                NameEn = $"# Men {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-maleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsMaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Männer {departmentVbs}",
                NameFr = $"# Hommes {departmentVbs}",
                NameIt = $"# Uomini {departmentVbs}",
                NameEn = $"# Men {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-maleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdMaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Männer {departmentEfd}",
                NameFr = $"# Hommes {departmentEfd}",
                NameIt = $"# Uomini {departmentEfd}",
                NameEn = $"# Men {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-maleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfMaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Männer {departmentWbf}",
                NameFr = $"# Hommes {departmentWbf}",
                NameIt = $"# Uomini {departmentWbf}",
                NameEn = $"# Men {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-maleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekMaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Männer {departmentUvek}",
                NameFr = $"# Hommes {departmentUvek}",
                NameIt = $"# Uomini {departmentUvek}",
                NameEn = $"# Men {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-femalePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaFemalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Frauen {departmentEda}",
                NameFr = $"% Femmes {departmentEda}",
                NameIt = $"% Donne {departmentEda}",
                NameEn = $"% Women {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-femalePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiFemalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Frauen {departmentEdi}",
                NameFr = $"% Femmes {departmentEdi}",
                NameIt = $"% Donne {departmentEdi}",
                NameEn = $"% Women {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-femalePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdFemalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Frauen {departmentEjpd}",
                NameFr = $"% Femmes {departmentEjpd}",
                NameIt = $"% Donne {departmentEjpd}",
                NameEn = $"% Women {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-femalePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsFemalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Frauen {departmentVbs}",
                NameFr = $"% Femmes {departmentVbs}",
                NameIt = $"% Donne {departmentVbs}",
                NameEn = $"% Women {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-femalePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdFemalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Frauen {departmentEfd}",
                NameFr = $"% Femmes {departmentEfd}",
                NameIt = $"% Donne {departmentEfd}",
                NameEn = $"% Women {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-femalePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfFemalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Frauen {departmentWbf}",
                NameFr = $"% Femmes {departmentWbf}",
                NameIt = $"% Donne {departmentWbf}",
                NameEn = $"% Women {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-femalePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekFemalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Frauen {departmentUvek}",
                NameFr = $"% Femmes {departmentUvek}",
                NameIt = $"% Donne {departmentUvek}",
                NameEn = $"% Women {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-malePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaMalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Männer {departmentEda}",
                NameFr = $"% Hommes {departmentEda}",
                NameIt = $"% Uomini {departmentEda}",
                NameEn = $"% Men {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-malePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiMalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Männer {departmentEdi}",
                NameFr = $"% Hommes {departmentEdi}",
                NameIt = $"% Uomini {departmentEdi}",
                NameEn = $"% Men {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-malePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdMalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Männer {departmentEjpd}",
                NameFr = $"% Hommes {departmentEjpd}",
                NameIt = $"% Uomini {departmentEjpd}",
                NameEn = $"% Men {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-malePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsMalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Männer {departmentVbs}",
                NameFr = $"% Hommes {departmentVbs}",
                NameIt = $"% Uomini {departmentVbs}",
                NameEn = $"% Men {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-malePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdMalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Männer {departmentEfd}",
                NameFr = $"% Hommes {departmentEfd}",
                NameIt = $"% Uomini {departmentEfd}",
                NameEn = $"% Men {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-malePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfMalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Männer {departmentWbf}",
                NameFr = $"% Hommes {departmentWbf}",
                NameIt = $"% Uomini {departmentWbf}",
                NameEn = $"% Men {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-malePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekMalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Männer {departmentUvek}",
                NameFr = $"% Hommes {departmentUvek}",
                NameIt = $"% Uomini {departmentUvek}",
                NameEn = $"% Men {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-femaleCount",
            Object = statisticDto.AuthoritiesCommissionsFemaleCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-maleCount",
            Object = statisticDto.AuthoritiesCommissionsMaleCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-femalePercentage",
            Object = statisticDto.AuthoritiesCommissionsFemalePercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-malePercentage",
            Object = statisticDto.AuthoritiesCommissionsMalePercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-femaleCount",
            Object = statisticDto.AdministrationCommissionsFemaleCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-maleCount",
            Object = statisticDto.AdministrationCommissionsMaleCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-femalePercentage",
            Object = statisticDto.AdministrationCommissionsFemalePercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-malePercentage",
            Object = statisticDto.AdministrationCommissionsMalePercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-germanCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaGermanCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Deutsch {departmentEda}",
                NameFr = $"# Allemand {departmentEda}",
                NameIt = $"# Tedesco {departmentEda}",
                NameEn = $"# German {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-germanCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiGermanCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Deutsch {departmentEdi}",
                NameFr = $"# Allemand {departmentEdi}",
                NameIt = $"# Tedesco {departmentEdi}",
                NameEn = $"# German {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-germanCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdGermanCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Deutsch {departmentEjpd}",
                NameFr = $"# Allemand {departmentEjpd}",
                NameIt = $"# Tedesco {departmentEjpd}",
                NameEn = $"# German {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-germanCount",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsGermanCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Deutsch {departmentVbs}",
                NameFr = $"# Allemand {departmentVbs}",
                NameIt = $"# Tedesco {departmentVbs}",
                NameEn = $"# German {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-germanCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdGermanCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Deutsch {departmentEfd}",
                NameFr = $"# Allemand {departmentEfd}",
                NameIt = $"# Tedesco {departmentEfd}",
                NameEn = $"# German {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-germanCount",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfGermanCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Deutsch {departmentWbf}",
                NameFr = $"# Allemand {departmentWbf}",
                NameIt = $"# Tedesco {departmentWbf}",
                NameEn = $"# German {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-germanCount",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekGermanCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Deutsch {departmentUvek}",
                NameFr = $"# Allemand {departmentUvek}",
                NameIt = $"# Tedesco {departmentUvek}",
                NameEn = $"# German {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-germanPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaGermanPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Deutsch {departmentEda}",
                NameFr = $"% Allemand {departmentEda}",
                NameIt = $"% Tedesco {departmentEda}",
                NameEn = $"% German {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-germanPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiGermanPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Deutsch {departmentEdi}",
                NameFr = $"% Allemand {departmentEdi}",
                NameIt = $"% Tedesco {departmentEdi}",
                NameEn = $"% German {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-germanPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdGermanPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Deutsch {departmentEjpd}",
                NameFr = $"% Allemand {departmentEjpd}",
                NameIt = $"% Tedesco {departmentEjpd}",
                NameEn = $"% German {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-germanPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsGermanPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Deutsch {departmentVbs}",
                NameFr = $"% Allemand {departmentVbs}",
                NameIt = $"% Tedesco {departmentVbs}",
                NameEn = $"% German {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-germanPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdGermanPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Deutsch {departmentEfd}",
                NameFr = $"% Allemand {departmentEfd}",
                NameIt = $"% Tedesco {departmentEfd}",
                NameEn = $"% German {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-germanPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfGermanPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Deutsch {departmentWbf}",
                NameFr = $"% Allemand {departmentWbf}",
                NameIt = $"% Tedesco {departmentWbf}",
                NameEn = $"% German {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-germanPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekGermanPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Deutsch {departmentUvek}",
                NameFr = $"% Allemand {departmentUvek}",
                NameIt = $"% Tedesco {departmentUvek}",
                NameEn = $"% German {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-frenchCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaFrenchCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Französisch {departmentEda}",
                NameFr = $"# Français {departmentEda}",
                NameIt = $"# Francese {departmentEda}",
                NameEn = $"# French {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-frenchCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiFrenchCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Französisch {departmentEdi}",
                NameFr = $"# Français {departmentEdi}",
                NameIt = $"# Francese {departmentEdi}",
                NameEn = $"# French {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-frenchCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdFrenchCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Französisch {departmentEjpd}",
                NameFr = $"# Français {departmentEjpd}",
                NameIt = $"# Francese {departmentEjpd}",
                NameEn = $"# French {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-frenchCount",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsFrenchCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Französisch {departmentVbs}",
                NameFr = $"# Français {departmentVbs}",
                NameIt = $"# Francese {departmentVbs}",
                NameEn = $"# French {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-frenchCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdFrenchCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Französisch {departmentEfd}",
                NameFr = $"# Français {departmentEfd}",
                NameIt = $"# Francese {departmentEfd}",
                NameEn = $"# French {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-frenchCount",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfFrenchCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Französisch {departmentWbf}",
                NameFr = $"# Français {departmentWbf}",
                NameIt = $"# Francese {departmentWbf}",
                NameEn = $"# French {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-frenchCount",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekFrenchCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Französisch {departmentUvek}",
                NameFr = $"# Français {departmentUvek}",
                NameIt = $"# Francese {departmentUvek}",
                NameEn = $"# French {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-frenchPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaFrenchPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Französisch {departmentEda}",
                NameFr = $"% Français {departmentEda}",
                NameIt = $"% Francese {departmentEda}",
                NameEn = $"% French {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-frenchPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiFrenchPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Französisch {departmentEdi}",
                NameFr = $"% Français {departmentEdi}",
                NameIt = $"% Francese {departmentEdi}",
                NameEn = $"% French {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-frenchPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdFrenchPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Französisch {departmentEjpd}",
                NameFr = $"% Français {departmentEjpd}",
                NameIt = $"% Francese {departmentEjpd}",
                NameEn = $"% French {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-frenchPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsFrenchPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Französisch {departmentVbs}",
                NameFr = $"% Français {departmentVbs}",
                NameIt = $"% Francese {departmentVbs}",
                NameEn = $"% French {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-frenchPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdFrenchPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Französisch {departmentEfd}",
                NameFr = $"% Français {departmentEfd}",
                NameIt = $"% Francese {departmentEfd}",
                NameEn = $"% French {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-frenchPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfFrenchPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Französisch {departmentWbf}",
                NameFr = $"% Français {departmentWbf}",
                NameIt = $"% Francese {departmentWbf}",
                NameEn = $"% French {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-frenchPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekFrenchPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Französisch {departmentUvek}",
                NameFr = $"% Français {departmentUvek}",
                NameIt = $"% Francese {departmentUvek}",
                NameEn = $"% French {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-italianCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaItalianCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Italienisch {departmentEda}",
                NameFr = $"# Italien {departmentEda}",
                NameIt = $"# Italiano {departmentEda}",
                NameEn = $"# Italian {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-italianCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiItalianCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Italienisch {departmentEdi}",
                NameFr = $"# Italien {departmentEdi}",
                NameIt = $"# Italiano {departmentEdi}",
                NameEn = $"# Italian {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-italianCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdItalianCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Italienisch {departmentEjpd}",
                NameFr = $"# Italien {departmentEjpd}",
                NameIt = $"# Italiano {departmentEjpd}",
                NameEn = $"# Italian {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-italianCount",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsItalianCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Italienisch {departmentVbs}",
                NameFr = $"# Italien {departmentVbs}",
                NameIt = $"# Italiano {departmentVbs}",
                NameEn = $"# Italian {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-italianCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdItalianCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Italienisch {departmentEfd}",
                NameFr = $"# Italien {departmentEfd}",
                NameIt = $"# Italiano {departmentEfd}",
                NameEn = $"# Italian {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-italianCount",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfItalianCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Italienisch {departmentWbf}",
                NameFr = $"# Italien {departmentWbf}",
                NameIt = $"# Italiano {departmentWbf}",
                NameEn = $"# Italian {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-italianCount",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekItalianCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Italienisch {departmentUvek}",
                NameFr = $"# Italien {departmentUvek}",
                NameIt = $"# Italiano {departmentUvek}",
                NameEn = $"# Italian {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-italianPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaItalianPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Italienisch {departmentEda}",
                NameFr = $"% Italien {departmentEda}",
                NameIt = $"% Italiano {departmentEda}",
                NameEn = $"% Italian {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-italianPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiItalianPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Italienisch {departmentEdi}",
                NameFr = $"% Italien {departmentEdi}",
                NameIt = $"% Italiano {departmentEdi}",
                NameEn = $"% Italian {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-italianPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdItalianPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Italienisch {departmentEjpd}",
                NameFr = $"% Italien {departmentEjpd}",
                NameIt = $"% Italiano {departmentEjpd}",
                NameEn = $"% Italian {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-italianPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsItalianPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Italienisch {departmentVbs}",
                NameFr = $"% Italien {departmentVbs}",
                NameIt = $"% Italiano {departmentVbs}",
                NameEn = $"% Italian {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-italianPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdItalianPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Italienisch {departmentEfd}",
                NameFr = $"% Italien {departmentEfd}",
                NameIt = $"% Italiano {departmentEfd}",
                NameEn = $"% Italian {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-italianPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfItalianPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Italienisch {departmentWbf}",
                NameFr = $"% Italien {departmentWbf}",
                NameIt = $"% Italiano {departmentWbf}",
                NameEn = $"% Italian {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-italianPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekItalianPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Italienisch {departmentUvek}",
                NameFr = $"% Italien {departmentUvek}",
                NameIt = $"% Italiano {departmentUvek}",
                NameEn = $"% Italian {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-romanshCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaRomanshCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Rätoromanisch {departmentEda}",
                NameFr = $"# Romanche {departmentEda}",
                NameIt = $"# Romancio {departmentEda}",
                NameEn = $"# Romansh {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-romanshCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiRomanshCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Rätoromanisch {departmentEdi}",
                NameFr = $"# Romanche {departmentEdi}",
                NameIt = $"# Romancio {departmentEdi}",
                NameEn = $"# Romansh {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-romanshCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdRomanshCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Rätoromanisch {departmentEjpd}",
                NameFr = $"# Romanche {departmentEjpd}",
                NameIt = $"# Romancio {departmentEjpd}",
                NameEn = $"# Romansh {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-romanshCount",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsRomanshCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Rätoromanisch {departmentVbs}",
                NameFr = $"# Romanche {departmentVbs}",
                NameIt = $"# Romancio {departmentVbs}",
                NameEn = $"# Romansh {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-romanshCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdRomanshCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Rätoromanisch {departmentEfd}",
                NameFr = $"# Romanche {departmentEfd}",
                NameIt = $"# Romancio {departmentEfd}",
                NameEn = $"# Romansh {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-romanshCount",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfRomanshCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Rätoromanisch {departmentWbf}",
                NameFr = $"# Romanche {departmentWbf}",
                NameIt = $"# Romancio {departmentWbf}",
                NameEn = $"# Romansh {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-romanshCount",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekRomanshCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Rätoromanisch {departmentUvek}",
                NameFr = $"# Romanche {departmentUvek}",
                NameIt = $"# Romancio {departmentUvek}",
                NameEn = $"# Romansh {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-romanshPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaRomanshPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Rätoromanisch {departmentEda}",
                NameFr = $"% Romanche {departmentEda}",
                NameIt = $"% Romancio {departmentEda}",
                NameEn = $"% Romansh {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-romanshPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiRomanshPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Rätoromanisch {departmentEdi}",
                NameFr = $"% Romanche {departmentEdi}",
                NameIt = $"% Romancio {departmentEdi}",
                NameEn = $"% Romansh {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-romanshPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdRomanshPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Rätoromanisch {departmentEjpd}",
                NameFr = $"% Romanche {departmentEjpd}",
                NameIt = $"% Romancio {departmentEjpd}",
                NameEn = $"% Romansh {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-romanshPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsRomanshPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Rätoromanisch {departmentVbs}",
                NameFr = $"% Romanche {departmentVbs}",
                NameIt = $"% Romancio {departmentVbs}",
                NameEn = $"% Romansh {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-romanshPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdRomanshPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Rätoromanisch {departmentEfd}",
                NameFr = $"% Romanche {departmentEfd}",
                NameIt = $"% Romancio {departmentEfd}",
                NameEn = $"% Romansh {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-romanshPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfRomanshPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Rätoromanisch {departmentWbf}",
                NameFr = $"% Romanche {departmentWbf}",
                NameIt = $"% Romancio {departmentWbf}",
                NameEn = $"% Romansh {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-romanshPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekRomanshPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Rätoromanisch {departmentUvek}",
                NameFr = $"% Romanche {departmentUvek}",
                NameIt = $"% Romancio {departmentUvek}",
                NameEn = $"% Romansh {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-germanCount",
            Object = statisticDto.AuthoritiesCommissionsGermanCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-frenchCount",
            Object = statisticDto.AuthoritiesCommissionsFrenchCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-italianCount",
            Object = statisticDto.AuthoritiesCommissionsItalianCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-romanshCount",
            Object = statisticDto.AuthoritiesCommissionsRomanshCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-germanPercentage",
            Object = statisticDto.AuthoritiesCommissionsGermanPercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-frenchPercentage",
            Object = statisticDto.AuthoritiesCommissionsFrenchPercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-italianPercentage",
            Object = statisticDto.AuthoritiesCommissionsItalianPercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-romanshPercentage",
            Object = statisticDto.AuthoritiesCommissionsRomanshPercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-germanCount",
            Object = statisticDto.AdministrationCommissionsGermanCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-frenchCount",
            Object = statisticDto.AdministrationCommissionsFrenchCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-italianCount",
            Object = statisticDto.AdministrationCommissionsItalianCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-romanshCount",
            Object = statisticDto.AdministrationCommissionsRomanshCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-germanPercentage",
            Object = statisticDto.AdministrationCommissionsGermanPercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-frenchPercentage",
            Object = statisticDto.AdministrationCommissionsFrenchPercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-italianPercentage",
            Object = statisticDto.AdministrationCommissionsItalianPercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-romanshPercentage",
            Object = statisticDto.AdministrationCommissionsRomanshPercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{apkNamespace}-total-femaleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsTotalFemaleCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{apkNamespace}-total-maleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsTotalMaleCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{apkNamespace}-total-femalePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsTotalFemalePercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{apkNamespace}-total-malePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsTotalMalePercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{apkNamespace}-total-germanCount",
            Object = statisticDto.ExtraParliamentaryCommissionsTotalFrenchCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{apkNamespace}-total-frenchCount",
            Object = statisticDto.ExtraParliamentaryCommissionsTotalFrenchCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{apkNamespace}-total-italianCount",
            Object = statisticDto.ExtraParliamentaryCommissionsTotalItalianCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{apkNamespace}-total-romanshCount",
            Object = statisticDto.ExtraParliamentaryCommissionsTotalRomanshCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{apkNamespace}-total-germanPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsTotalGermanPercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{apkNamespace}-total-frenchPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsTotalFrenchPercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{apkNamespace}-total-italianPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsTotalItalianPercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{apkNamespace}-total-romanshPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsTotalRomanshPercentage.ToString(CultureInfo.InvariantCulture),
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

        // 2nd Group of committees (non extra parliamentary commissions)

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-count",
            Object = statisticDto.NonExtraParliamentaryCommissionsCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "# Nicht ausserp Kommissionen",
                NameFr = "# Commissions non extraparlementaires",
                NameIt = "# Commissioni non extraparlamentari",
                NameEn = "# Non Extraparliamentary Commissions",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-count",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentEda}",
                NameFr = $"# {departmentEda}",
                NameIt = $"# {departmentEda}",
                NameEn = $"# {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-count",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentEdi}",
                NameFr = $"# {departmentEdi}",
                NameIt = $"# {departmentEdi}",
                NameEn = $"# {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-count",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentEjpd}",
                NameFr = $"# {departmentEjpd}",
                NameIt = $"# {departmentEjpd}",
                NameEn = $"# {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-count",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentVbs}",
                NameFr = $"# {departmentVbs}",
                NameIt = $"# {departmentVbs}",
                NameEn = $"# {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-count",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentEfd}",
                NameFr = $"# {departmentEfd}",
                NameIt = $"# {departmentEfd}",
                NameEn = $"# {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-count",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentWbf}",
                NameFr = $"# {departmentWbf}",
                NameIt = $"# {departmentWbf}",
                NameEn = $"# {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-count",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentUvek}",
                NameFr = $"# {departmentUvek}",
                NameIt = $"# {departmentUvek}",
                NameEn = $"# {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-count",
            Object = statisticDto.ManagementCommitteesCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "# Geschäftskommissionen",
                NameFr = "# Commissions de gestion",
                NameIt = "# Commissioni di gestione",
                NameEn = "# Management Commissions",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-count",
            Object = statisticDto.FederalAgenciesCommitteesCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "# Eidg. Anstalten Kommissionen",
                NameFr = "# Commissions des établissements fédéraux",
                NameIt = "# Commissioni degli stabilimenti federali",
                NameEn = "# Federal Agencies Commissions",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-{departmentEda}-count",
            Object = statisticDto.ManagementCommitteesEdaCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentEda}",
                NameFr = $"# {departmentEda}",
                NameIt = $"# {departmentEda}",
                NameEn = $"# {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-{departmentEdi}-count",
            Object = statisticDto.ManagementCommitteesEdiCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentEdi}",
                NameFr = $"# {departmentEdi}",
                NameIt = $"# {departmentEdi}",
                NameEn = $"# {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-{departmentEjpd}-count",
            Object = statisticDto.ManagementCommitteesEjpdCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentEjpd}",
                NameFr = $"# {departmentEjpd}",
                NameIt = $"# {departmentEjpd}",
                NameEn = $"# {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-{departmentVbs}-count",
            Object = statisticDto.ManagementCommitteesVbsCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentVbs}",
                NameFr = $"# {departmentVbs}",
                NameIt = $"# {departmentVbs}",
                NameEn = $"# {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-{departmentEfd}-count",
            Object = statisticDto.ManagementCommitteesEfdCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentEfd}",
                NameFr = $"# {departmentEfd}",
                NameIt = $"# {departmentEfd}",
                NameEn = $"# {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-{departmentWbf}-count",
            Object = statisticDto.ManagementCommitteesWbfCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentWbf}",
                NameFr = $"# {departmentWbf}",
                NameIt = $"# {departmentWbf}",
                NameEn = $"# {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-{departmentUvek}-count",
            Object = statisticDto.ManagementCommitteesUvekCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentUvek}",
                NameFr = $"# {departmentUvek}",
                NameIt = $"# {departmentUvek}",
                NameEn = $"# {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-{departmentEda}-count",
            Object = statisticDto.FederalAgenciesCommitteesEdaCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentEda}",
                NameFr = $"# {departmentEda}",
                NameIt = $"# {departmentEda}",
                NameEn = $"# {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-{departmentEdi}-count",
            Object = statisticDto.FederalAgenciesCommitteesEdiCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentEdi}",
                NameFr = $"# {departmentEdi}",
                NameIt = $"# {departmentEdi}",
                NameEn = $"# {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-{departmentEjpd}-count",
            Object = statisticDto.FederalAgenciesCommitteesEjpdCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentEjpd}",
                NameFr = $"# {departmentEjpd}",
                NameIt = $"# {departmentEjpd}",
                NameEn = $"# {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-{departmentVbs}-count",
            Object = statisticDto.FederalAgenciesCommitteesVbsCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentVbs}",
                NameFr = $"# {departmentVbs}",
                NameIt = $"# {departmentVbs}",
                NameEn = $"# {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-{departmentEfd}-count",
            Object = statisticDto.FederalAgenciesCommitteesEfdCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentEfd}",
                NameFr = $"# {departmentEfd}",
                NameIt = $"# {departmentEfd}",
                NameEn = $"# {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-{departmentWbf}-count",
            Object = statisticDto.FederalAgenciesCommitteesWbfCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentWbf}",
                NameFr = $"# {departmentWbf}",
                NameIt = $"# {departmentWbf}",
                NameEn = $"# {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-{departmentUvek}-count",
            Object = statisticDto.FederalAgenciesCommitteesUvekCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# {departmentUvek}",
                NameFr = $"# {departmentUvek}",
                NameIt = $"# {departmentUvek}",
                NameEn = $"# {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-femaleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaFemaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Frauen {departmentEda}",
                NameFr = $"# Femmes {departmentEda}",
                NameIt = $"# Donne {departmentEda}",
                NameEn = $"# Women {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-femaleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiFemaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Frauen {departmentEdi}",
                NameFr = $"# Femmes {departmentEdi}",
                NameIt = $"# Donne {departmentEdi}",
                NameEn = $"# Women {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-femaleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdFemaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Frauen {departmentEjpd}",
                NameFr = $"# Femmes {departmentEjpd}",
                NameIt = $"# Donne {departmentEjpd}",
                NameEn = $"# Women {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-femaleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsFemaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Frauen {departmentVbs}",
                NameFr = $"# Femmes {departmentVbs}",
                NameIt = $"# Donne {departmentVbs}",
                NameEn = $"# Women {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-femaleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdFemaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Frauen {departmentEfd}",
                NameFr = $"# Femmes {departmentEfd}",
                NameIt = $"# Donne {departmentEfd}",
                NameEn = $"# Women {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-femaleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfFemaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Frauen {departmentWbf}",
                NameFr = $"# Femmes {departmentWbf}",
                NameIt = $"# Donne {departmentWbf}",
                NameEn = $"# Women {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-femaleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekFemaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Frauen {departmentUvek}",
                NameFr = $"# Femmes {departmentUvek}",
                NameIt = $"# Donne {departmentUvek}",
                NameEn = $"# Women {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-maleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaMaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Männer {departmentEda}",
                NameFr = $"# Hommes {departmentEda}",
                NameIt = $"# Uomini {departmentEda}",
                NameEn = $"# Men {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-maleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiMaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Männer {departmentEdi}",
                NameFr = $"# Hommes {departmentEdi}",
                NameIt = $"# Uomini {departmentEdi}",
                NameEn = $"# Men {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-maleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdMaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Männer {departmentEjpd}",
                NameFr = $"# Hommes {departmentEjpd}",
                NameIt = $"# Uomini {departmentEjpd}",
                NameEn = $"# Men {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-maleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsMaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Männer {departmentVbs}",
                NameFr = $"# Hommes {departmentVbs}",
                NameIt = $"# Uomini {departmentVbs}",
                NameEn = $"# Men {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-maleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdMaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Männer {departmentEfd}",
                NameFr = $"# Hommes {departmentEfd}",
                NameIt = $"# Uomini {departmentEfd}",
                NameEn = $"# Men {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-maleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfMaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Männer {departmentWbf}",
                NameFr = $"# Hommes {departmentWbf}",
                NameIt = $"# Uomini {departmentWbf}",
                NameEn = $"# Men {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-maleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekMaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Männer {departmentUvek}",
                NameFr = $"# Hommes {departmentUvek}",
                NameIt = $"# Uomini {departmentUvek}",
                NameEn = $"# Men {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-femalePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaFemalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Frauen {departmentEda}",
                NameFr = $"% Femmes {departmentEda}",
                NameIt = $"% Donne {departmentEda}",
                NameEn = $"% Women {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-femalePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiFemalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Frauen {departmentEdi}",
                NameFr = $"% Femmes {departmentEdi}",
                NameIt = $"% Donne {departmentEdi}",
                NameEn = $"% Women {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-femalePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdFemalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Frauen {departmentEjpd}",
                NameFr = $"% Femmes {departmentEjpd}",
                NameIt = $"% Donne {departmentEjpd}",
                NameEn = $"% Women {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-femalePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsFemalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Frauen {departmentVbs}",
                NameFr = $"% Femmes {departmentVbs}",
                NameIt = $"% Donne {departmentVbs}",
                NameEn = $"% Women {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-femalePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdFemalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Frauen {departmentEfd}",
                NameFr = $"% Femmes {departmentEfd}",
                NameIt = $"% Donne {departmentEfd}",
                NameEn = $"% Women {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-femalePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfFemalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Frauen {departmentWbf}",
                NameFr = $"% Femmes {departmentWbf}",
                NameIt = $"% Donne {departmentWbf}",
                NameEn = $"% Women {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-femalePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekFemalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Frauen {departmentUvek}",
                NameFr = $"% Femmes {departmentUvek}",
                NameIt = $"% Donne {departmentUvek}",
                NameEn = $"% Women {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-malePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaMalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Männer {departmentEda}",
                NameFr = $"% Hommes {departmentEda}",
                NameIt = $"% Uomini {departmentEda}",
                NameEn = $"% Men {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-malePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiMalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Männer {departmentEdi}",
                NameFr = $"% Hommes {departmentEdi}",
                NameIt = $"% Uomini {departmentEdi}",
                NameEn = $"% Men {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-malePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdMalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Männer {departmentEjpd}",
                NameFr = $"% Hommes {departmentEjpd}",
                NameIt = $"% Uomini {departmentEjpd}",
                NameEn = $"% Men {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-malePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsMalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Männer {departmentVbs}",
                NameFr = $"% Hommes {departmentVbs}",
                NameIt = $"% Uomini {departmentVbs}",
                NameEn = $"% Men {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-malePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdMalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Männer {departmentEfd}",
                NameFr = $"% Hommes {departmentEfd}",
                NameIt = $"% Uomini {departmentEfd}",
                NameEn = $"% Men {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-malePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfMalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Männer {departmentWbf}",
                NameFr = $"% Hommes {departmentWbf}",
                NameIt = $"% Uomini {departmentWbf}",
                NameEn = $"% Men {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-malePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekMalePercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Männer {departmentUvek}",
                NameFr = $"% Hommes {departmentUvek}",
                NameIt = $"% Uomini {departmentUvek}",
                NameEn = $"% Men {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-femaleCount",
            Object = statisticDto.ManagementCommitteesFemaleCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-maleCount",
            Object = statisticDto.ManagementCommitteesMaleCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-femalePercentage",
            Object = statisticDto.ManagementCommitteesFemalePercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-malePercentage",
            Object = statisticDto.ManagementCommitteesMalePercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-femaleCount",
            Object = statisticDto.FederalAgenciesCommitteesFemaleCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-maleCount",
            Object = statisticDto.FederalAgenciesCommitteesMaleCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-femalePercentage",
            Object = statisticDto.FederalAgenciesCommitteesFemalePercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-malePercentage",
            Object = statisticDto.FederalAgenciesCommitteesMalePercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-germanCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaGermanCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Deutsch {departmentEda}",
                NameFr = $"# Allemand {departmentEda}",
                NameIt = $"# Tedesco {departmentEda}",
                NameEn = $"# German {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-germanCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiGermanCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Deutsch {departmentEdi}",
                NameFr = $"# Allemand {departmentEdi}",
                NameIt = $"# Tedesco {departmentEdi}",
                NameEn = $"# German {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-germanCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdGermanCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Deutsch {departmentEjpd}",
                NameFr = $"# Allemand {departmentEjpd}",
                NameIt = $"# Tedesco {departmentEjpd}",
                NameEn = $"# German {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-germanCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsGermanCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Deutsch {departmentVbs}",
                NameFr = $"# Allemand {departmentVbs}",
                NameIt = $"# Tedesco {departmentVbs}",
                NameEn = $"# German {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-germanCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdGermanCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Deutsch {departmentEfd}",
                NameFr = $"# Allemand {departmentEfd}",
                NameIt = $"# Tedesco {departmentEfd}",
                NameEn = $"# German {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-germanCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfGermanCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Deutsch {departmentWbf}",
                NameFr = $"# Allemand {departmentWbf}",
                NameIt = $"# Tedesco {departmentWbf}",
                NameEn = $"# German {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-germanCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekGermanCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Deutsch {departmentUvek}",
                NameFr = $"# Allemand {departmentUvek}",
                NameIt = $"# Tedesco {departmentUvek}",
                NameEn = $"# German {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-germanPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaGermanPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Deutsch {departmentEda}",
                NameFr = $"% Allemand {departmentEda}",
                NameIt = $"% Tedesco {departmentEda}",
                NameEn = $"% German {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-germanPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiGermanPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Deutsch {departmentEdi}",
                NameFr = $"% Allemand {departmentEdi}",
                NameIt = $"% Tedesco {departmentEdi}",
                NameEn = $"% German {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-germanPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdGermanPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Deutsch {departmentEjpd}",
                NameFr = $"% Allemand {departmentEjpd}",
                NameIt = $"% Tedesco {departmentEjpd}",
                NameEn = $"% German {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-germanPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsGermanPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Deutsch {departmentVbs}",
                NameFr = $"% Allemand {departmentVbs}",
                NameIt = $"% Tedesco {departmentVbs}",
                NameEn = $"% German {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-germanPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdGermanPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Deutsch {departmentEfd}",
                NameFr = $"% Allemand {departmentEfd}",
                NameIt = $"% Tedesco {departmentEfd}",
                NameEn = $"% German {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-germanPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfGermanPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Deutsch {departmentWbf}",
                NameFr = $"% Allemand {departmentWbf}",
                NameIt = $"% Tedesco {departmentWbf}",
                NameEn = $"% German {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-germanPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekGermanPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Deutsch {departmentUvek}",
                NameFr = $"% Allemand {departmentUvek}",
                NameIt = $"% Tedesco {departmentUvek}",
                NameEn = $"% German {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-frenchCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaFrenchCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Französisch {departmentEda}",
                NameFr = $"# Français {departmentEda}",
                NameIt = $"# Francese {departmentEda}",
                NameEn = $"# French {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-frenchCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiFrenchCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Französisch {departmentEdi}",
                NameFr = $"# Français {departmentEdi}",
                NameIt = $"# Francese {departmentEdi}",
                NameEn = $"# French {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-frenchCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdFrenchCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Französisch {departmentEjpd}",
                NameFr = $"# Français {departmentEjpd}",
                NameIt = $"# Francese {departmentEjpd}",
                NameEn = $"# French {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-frenchCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsFrenchCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Französisch {departmentVbs}",
                NameFr = $"# Français {departmentVbs}",
                NameIt = $"# Francese {departmentVbs}",
                NameEn = $"# French {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-frenchCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdFrenchCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Französisch {departmentEfd}",
                NameFr = $"# Français {departmentEfd}",
                NameIt = $"# Francese {departmentEfd}",
                NameEn = $"# French {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-frenchCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfFrenchCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Französisch {departmentWbf}",
                NameFr = $"# Français {departmentWbf}",
                NameIt = $"# Francese {departmentWbf}",
                NameEn = $"# French {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-frenchCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekFrenchCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Französisch {departmentUvek}",
                NameFr = $"# Français {departmentUvek}",
                NameIt = $"# Francese {departmentUvek}",
                NameEn = $"# French {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-frenchPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaFrenchPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Französisch {departmentEda}",
                NameFr = $"% Français {departmentEda}",
                NameIt = $"% Francese {departmentEda}",
                NameEn = $"% French {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-frenchPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiFrenchPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Französisch {departmentEdi}",
                NameFr = $"% Français {departmentEdi}",
                NameIt = $"% Francese {departmentEdi}",
                NameEn = $"% French {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-frenchPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdFrenchPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Französisch {departmentEjpd}",
                NameFr = $"% Français {departmentEjpd}",
                NameIt = $"% Francese {departmentEjpd}",
                NameEn = $"% French {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-frenchPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsFrenchPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Französisch {departmentVbs}",
                NameFr = $"% Français {departmentVbs}",
                NameIt = $"% Francese {departmentVbs}",
                NameEn = $"% French {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-frenchPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdFrenchPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Französisch {departmentEfd}",
                NameFr = $"% Français {departmentEfd}",
                NameIt = $"% Francese {departmentEfd}",
                NameEn = $"% French {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-frenchPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfFrenchPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Französisch {departmentWbf}",
                NameFr = $"% Français {departmentWbf}",
                NameIt = $"% Francese {departmentWbf}",
                NameEn = $"% French {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-frenchPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekFrenchPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Französisch {departmentUvek}",
                NameFr = $"% Français {departmentUvek}",
                NameIt = $"% Francese {departmentUvek}",
                NameEn = $"% French {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-italianCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaItalianCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Italienisch {departmentEda}",
                NameFr = $"# Italien {departmentEda}",
                NameIt = $"# Italiano {departmentEda}",
                NameEn = $"# Italian {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-italianCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiItalianCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Italienisch {departmentEdi}",
                NameFr = $"# Italien {departmentEdi}",
                NameIt = $"# Italiano {departmentEdi}",
                NameEn = $"# Italian {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-italianCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdItalianCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Italienisch {departmentEjpd}",
                NameFr = $"# Italien {departmentEjpd}",
                NameIt = $"# Italiano {departmentEjpd}",
                NameEn = $"# Italian {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-italianCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsItalianCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Italienisch {departmentVbs}",
                NameFr = $"# Italien {departmentVbs}",
                NameIt = $"# Italiano {departmentVbs}",
                NameEn = $"# Italian {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-italianCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdItalianCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Italienisch {departmentEfd}",
                NameFr = $"# Italien {departmentEfd}",
                NameIt = $"# Italiano {departmentEfd}",
                NameEn = $"# Italian {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-italianCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfItalianCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Italienisch {departmentWbf}",
                NameFr = $"# Italien {departmentWbf}",
                NameIt = $"# Italiano {departmentWbf}",
                NameEn = $"# Italian {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-italianCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekItalianCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Italienisch {departmentUvek}",
                NameFr = $"# Italien {departmentUvek}",
                NameIt = $"# Italiano {departmentUvek}",
                NameEn = $"# Italian {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-italianPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaItalianPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Italienisch {departmentEda}",
                NameFr = $"% Italien {departmentEda}",
                NameIt = $"% Italiano {departmentEda}",
                NameEn = $"% Italian {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-italianPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiItalianPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Italienisch {departmentEdi}",
                NameFr = $"% Italien {departmentEdi}",
                NameIt = $"% Italiano {departmentEdi}",
                NameEn = $"% Italian {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-italianPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdItalianPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Italienisch {departmentEjpd}",
                NameFr = $"% Italien {departmentEjpd}",
                NameIt = $"% Italiano {departmentEjpd}",
                NameEn = $"% Italian {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-italianPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsItalianPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Italienisch {departmentVbs}",
                NameFr = $"% Italien {departmentVbs}",
                NameIt = $"% Italiano {departmentVbs}",
                NameEn = $"% Italian {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-italianPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdItalianPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Italienisch {departmentEfd}",
                NameFr = $"% Italien {departmentEfd}",
                NameIt = $"% Italiano {departmentEfd}",
                NameEn = $"% Italian {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-italianPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfItalianPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Italienisch {departmentWbf}",
                NameFr = $"% Italien {departmentWbf}",
                NameIt = $"% Italiano {departmentWbf}",
                NameEn = $"% Italian {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-italianPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekItalianPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Italienisch {departmentUvek}",
                NameFr = $"% Italien {departmentUvek}",
                NameIt = $"% Italiano {departmentUvek}",
                NameEn = $"% Italian {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-romanshCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaRomanshCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Rätoromanisch {departmentEda}",
                NameFr = $"# Romanche {departmentEda}",
                NameIt = $"# Romancio {departmentEda}",
                NameEn = $"# Romansh {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-romanshCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiRomanshCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Rätoromanisch {departmentEdi}",
                NameFr = $"# Romanche {departmentEdi}",
                NameIt = $"# Romancio {departmentEdi}",
                NameEn = $"# Romansh {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-romanshCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdRomanshCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Rätoromanisch {departmentEjpd}",
                NameFr = $"# Romanche {departmentEjpd}",
                NameIt = $"# Romancio {departmentEjpd}",
                NameEn = $"# Romansh {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-romanshCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsRomanshCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Rätoromanisch {departmentVbs}",
                NameFr = $"# Romanche {departmentVbs}",
                NameIt = $"# Romancio {departmentVbs}",
                NameEn = $"# Romansh {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-romanshCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdRomanshCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Rätoromanisch {departmentEfd}",
                NameFr = $"# Romanche {departmentEfd}",
                NameIt = $"# Romancio {departmentEfd}",
                NameEn = $"# Romansh {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-romanshCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfRomanshCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Rätoromanisch {departmentWbf}",
                NameFr = $"# Romanche {departmentWbf}",
                NameIt = $"# Romancio {departmentWbf}",
                NameEn = $"# Romansh {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-romanshCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekRomanshCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"# Rätoromanisch {departmentUvek}",
                NameFr = $"# Romanche {departmentUvek}",
                NameIt = $"# Romancio {departmentUvek}",
                NameEn = $"# Romansh {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-romanshPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaRomanshPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Rätoromanisch {departmentEda}",
                NameFr = $"% Romanche {departmentEda}",
                NameIt = $"% Romancio {departmentEda}",
                NameEn = $"% Romansh {departmentEda}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-romanshPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiRomanshPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Rätoromanisch {departmentEdi}",
                NameFr = $"% Romanche {departmentEdi}",
                NameIt = $"% Romancio {departmentEdi}",
                NameEn = $"% Romansh {departmentEdi}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-romanshPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdRomanshPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Rätoromanisch {departmentEjpd}",
                NameFr = $"% Romanche {departmentEjpd}",
                NameIt = $"% Romancio {departmentEjpd}",
                NameEn = $"% Romansh {departmentEjpd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-romanshPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsRomanshPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Rätoromanisch {departmentVbs}",
                NameFr = $"% Romanche {departmentVbs}",
                NameIt = $"% Romancio {departmentVbs}",
                NameEn = $"% Romansh {departmentVbs}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-romanshPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdRomanshPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Rätoromanisch {departmentEfd}",
                NameFr = $"% Romanche {departmentEfd}",
                NameIt = $"% Romancio {departmentEfd}",
                NameEn = $"% Romansh {departmentEfd}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-romanshPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfRomanshPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Rätoromanisch {departmentWbf}",
                NameFr = $"% Romanche {departmentWbf}",
                NameIt = $"% Romancio {departmentWbf}",
                NameEn = $"% Romansh {departmentWbf}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-romanshPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekRomanshPercentage.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeDecimal,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = $"% Rätoromanisch {departmentUvek}",
                NameFr = $"% Romanche {departmentUvek}",
                NameIt = $"% Romancio {departmentUvek}",
                NameEn = $"% Romansh {departmentUvek}",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-germanCount",
            Object = statisticDto.ManagementCommitteesGermanCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-frenchCount",
            Object = statisticDto.ManagementCommitteesFrenchCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-italianCount",
            Object = statisticDto.ManagementCommitteesItalianCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-romanshCount",
            Object = statisticDto.ManagementCommitteesRomanshCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-germanPercentage",
            Object = statisticDto.ManagementCommitteesGermanPercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-frenchPercentage",
            Object = statisticDto.ManagementCommitteesFrenchPercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-italianPercentage",
            Object = statisticDto.ManagementCommitteesItalianPercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-romanshPercentage",
            Object = statisticDto.ManagementCommitteesRomanshPercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-germanCount",
            Object = statisticDto.FederalAgenciesCommitteesGermanCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-frenchCount",
            Object = statisticDto.FederalAgenciesCommitteesFrenchCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-italianCount",
            Object = statisticDto.FederalAgenciesCommitteesItalianCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-romanshCount",
            Object = statisticDto.FederalAgenciesCommitteesRomanshCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-germanPercentage",
            Object = statisticDto.FederalAgenciesCommitteesGermanPercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-frenchPercentage",
            Object = statisticDto.FederalAgenciesCommitteesFrenchPercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-italianPercentage",
            Object = statisticDto.FederalAgenciesCommitteesItalianPercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-romanshPercentage",
            Object = statisticDto.FederalAgenciesCommitteesRomanshPercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-total-femaleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsTotalFemaleCount.ToString(CultureInfo.InvariantCulture),
            DataTypeUri = OgdExportConstants.DataTypeInt,
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
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-total-maleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsTotalMaleCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-total-femalePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsTotalFemalePercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-total-malePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsTotalMalePercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-total-germanCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsTotalFrenchCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-total-frenchCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsTotalFrenchCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-total-italianCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsTotalItalianCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-total-romanshCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsTotalRomanshCount.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-total-germanPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsTotalGermanPercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-total-frenchPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsTotalFrenchPercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-total-italianPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsTotalItalianPercentage.ToString(CultureInfo.InvariantCulture),
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
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-total-romanshPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsTotalRomanshPercentage.ToString(CultureInfo.InvariantCulture),
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

        return dataRow;
    }
}
