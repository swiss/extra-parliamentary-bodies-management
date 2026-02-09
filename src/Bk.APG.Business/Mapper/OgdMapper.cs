using Bk.APG.Business.Dtos;
using Bk.APG.CrossCutting;
using Swiss.FCh.Cube.RawData.Model;

namespace Bk.APG.Business.Mapper;

public static class OgdMapper
{
    public static ObservationDataRow ToFunctionStatisticObservation(MembershipFunctionStatisticDto statisticDto)
    {
        var ogdId = "TOTAL";
        var ogdNamespace = OgdExportConstants.NamespaceCommitteeFunctionStatistic;

        if (statisticDto.FunctionOgdId != 0)
        {
            ogdId = statisticDto.FunctionOgdId.ToString();
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
            Object = statisticDto.FunctionCount.ToString(),
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "Anzahl Funktionen",
                NameFr = "FR_Anzahl Funktionen",
                NameIt = "IT_Anzahl Funktionen",
                NameEn = "EN_Anzahl Funktionen",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        return dataRow;
    }

    public static ObservationDataRow ToCantonStatisticObservation(MembershipCantonStatisticDto statisticDto)
    {
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
            Object = statisticDto.CantonCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt,
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "Anzahl Kantone",
                NameFr = "FR_Anzahl Kantone",
                NameIt = "IT_Anzahl Kantone",
                NameEn = "EN_Anzahl Kantone",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });
        return dataRow;
    }

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
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:femaleCount",
            Object = statisticDto.FemaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:femalePercentage",
            Object = statisticDto.FemalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:maleCount",
            Object = statisticDto.MaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:malePercentage",
            Object = statisticDto.MalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:germanCount",
            Object = statisticDto.GermanCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:germanPercentage",
            Object = statisticDto.GermanPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:frenchCount",
            Object = statisticDto.FrenchCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:frenchPercentage",
            Object = statisticDto.FrenchPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:italianCount",
            Object = statisticDto.ItalianCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:italianPercentage",
            Object = statisticDto.ItalianPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:romanshCount",
            Object = statisticDto.RomanshCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:romanshPercentage",
            Object = statisticDto.RomanshPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:federalDutyCount",
            Object = statisticDto.FederalDutyCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:federalAssemblyCount",
            Object = statisticDto.FederalAssemblyCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:upTo30Count",
            Object = statisticDto.UpTo30Count.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:upTo30Percentage",
            Object = statisticDto.UpTo30Percentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:from31to40Count",
            Object = statisticDto.From31To40Count.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:from31To40Percentage",
            Object = statisticDto.From31To40Percentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:from41to50Count",
            Object = statisticDto.From41To50Count.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:from41To50Percentage",
            Object = statisticDto.From41To50Percentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:from51to60Count",
            Object = statisticDto.From51To60Count.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:from51To60Percentage",
            Object = statisticDto.From51To60Percentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:from61to70Count",
            Object = statisticDto.From61To70Count.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:from61To70Percentage",
            Object = statisticDto.From61To70Percentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:over70Count",
            Object = statisticDto.Over70Count.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:over70Percentage",
            Object = statisticDto.Over70Percentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        return dataRow;
    }

    public static ObservationDataRow ToCantonDetailStatisticObservation(MembershipStatisticByCantonDto statisticDto)
    {
        var ogdNamespace = OgdExportConstants.NamespaceCommitteeCantonDetailStatistic;

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

        dataRow.KeyDimensionLinks.Add(new KeyDimensionLink
        {
            Predicate = $"{ogdNamespace}:hasCanton",
            Uri = $"canton:{statisticDto.CantonOgdId}"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:count",
            Object = statisticDto.CantonCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:femaleCount",
            Object = statisticDto.FemaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });
        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:femalePercentage",
            Object = statisticDto.FemalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:maleCount",
            Object = statisticDto.MaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:malePercentage",
            Object = statisticDto.MalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:germanCount",
            Object = statisticDto.GermanCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:germanPercentage",
            Object = statisticDto.GermanPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:frenchCount",
            Object = statisticDto.FrenchCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:frenchPercentage",
            Object = statisticDto.FrenchPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:italianCount",
            Object = statisticDto.ItalianCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:italianPercentage",
            Object = statisticDto.ItalianPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:romanshCount",
            Object = statisticDto.RomanshCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:romanshPercentage",
            Object = statisticDto.RomanshPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:federalDutyCount",
            Object = statisticDto.FederalDutyCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:federalAssemblyCount",
            Object = statisticDto.FederalAssemblyCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:upTo30Count",
            Object = statisticDto.UpTo30Count.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:upTo30Percentage",
            Object = statisticDto.UpTo30Percentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:from31to40Count",
            Object = statisticDto.From31To40Count.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:from31To40Percentage",
            Object = statisticDto.From31To40Percentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:from41to50Count",
            Object = statisticDto.From41To50Count.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:from41To50Percentage",
            Object = statisticDto.From41To50Percentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:from51to60Count",
            Object = statisticDto.From51To60Count.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:from51To60Percentage",
            Object = statisticDto.From51To60Percentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:from61to70Count",
            Object = statisticDto.From61To70Count.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:from61To70Percentage",
            Object = statisticDto.From61To70Percentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:over70Count",
            Object = statisticDto.Over70Count.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:over70Percentage",
            Object = statisticDto.Over70Percentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        return dataRow;
    }

    public static ObservationDataRow ToCommitteeTypeStatisticObservation(CommitteeTypeStatisticDto statisticDto)
    {
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
            Object = statisticDto.ExtraParliamentaryCommissionsCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-count",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-count",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-count",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-count",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-count",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-count",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-count",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-count",
            Object = statisticDto.AuthoritiesCommissionsCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-count",
            Object = statisticDto.AdministrationCommissionsCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-{departmentEda}-count",
            Object = statisticDto.AuthoritiesCommissionsEdaCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-{departmentEdi}-count",
            Object = statisticDto.AuthoritiesCommissionsEdiCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-{departmentEjpd}-count",
            Object = statisticDto.AuthoritiesCommissionsEjpdCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-{departmentVbs}-count",
            Object = statisticDto.AuthoritiesCommissionsVbsCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-{departmentEfd}-count",
            Object = statisticDto.AuthoritiesCommissionsEfdCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-{departmentWbf}-count",
            Object = statisticDto.AuthoritiesCommissionsWbfCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-{departmentUvek}-count",
            Object = statisticDto.AuthoritiesCommissionsUvekCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-{departmentEda}-count",
            Object = statisticDto.AdministrationCommissionsEdaCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-{departmentEdi}-count",
            Object = statisticDto.AdministrationCommissionsEdiCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-{departmentEjpd}-count",
            Object = statisticDto.AdministrationCommissionsEjpdCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-{departmentVbs}-count",
            Object = statisticDto.AdministrationCommissionsVbsCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-{departmentEfd}-count",
            Object = statisticDto.AdministrationCommissionsEfdCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-{departmentWbf}-count",
            Object = statisticDto.AdministrationCommissionsWbfCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-{departmentUvek}-count",
            Object = statisticDto.AdministrationCommissionsUvekCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-femaleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaFemaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-femaleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiFemaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-femaleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdFemaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-femaleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsFemaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-femaleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdFemaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-femaleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfFemaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-femaleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekFemaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-maleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaMaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-maleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiMaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-maleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdMaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-maleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsMaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-maleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdMaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-maleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfMaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-maleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekMaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-femalePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaFemalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-femalePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiFemalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-femalePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdFemalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-femalePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsFemalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-femalePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdFemalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-femalePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfFemalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-femalePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekFemalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-malePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaMalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-malePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiMalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-malePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdMalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-malePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsMalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-malePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdMalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-malePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfMalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-malePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekMalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-femaleCount",
            Object = statisticDto.AuthoritiesCommissionsFemaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-maleCount",
            Object = statisticDto.AuthoritiesCommissionsMaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-femalePercentage",
            Object = statisticDto.AuthoritiesCommissionsFemalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-malePercentage",
            Object = statisticDto.AuthoritiesCommissionsMalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-femaleCount",
            Object = statisticDto.AdministrationCommissionsFemaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-maleCount",
            Object = statisticDto.AdministrationCommissionsMaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-femalePercentage",
            Object = statisticDto.AdministrationCommissionsFemalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-malePercentage",
            Object = statisticDto.AdministrationCommissionsMalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-germanCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaGermanCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-germanCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiGermanCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-germanCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdGermanCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-germanCount",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsGermanCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-germanCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdGermanCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-germanCount",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfGermanCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-germanCount",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekGermanCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-germanPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaGermanPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-germanPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiGermanPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-germanPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdGermanPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-germanPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsGermanPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-germanPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdGermanPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-germanPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfGermanPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-germanPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekGermanPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-frenchCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaFrenchCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-frenchCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiFrenchCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-frenchCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdFrenchCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-frenchCount",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsFrenchCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-frenchCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdFrenchCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-frenchCount",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfFrenchCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-frenchCount",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekFrenchCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-frenchPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaFrenchPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-frenchPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiFrenchPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-frenchPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdFrenchPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-frenchPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsFrenchPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-frenchPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdFrenchPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-frenchPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfFrenchPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-frenchPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekFrenchPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-italianCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaItalianCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-italianCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiItalianCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-italianCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdItalianCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-italianCount",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsItalianCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-italianCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdItalianCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-italianCount",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfItalianCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-italianCount",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekItalianCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-italianPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaItalianPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-italianPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiItalianPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-italianPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdItalianPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-italianPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsItalianPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-italianPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdItalianPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-italianPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfItalianPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-italianPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekItalianPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-romanshCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaRomanshCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-romanshCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiRomanshCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-romanshCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdRomanshCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-romanshCount",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsRomanshCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-romanshCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdRomanshCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-romanshCount",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfRomanshCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-romanshCount",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekRomanshCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-romanshPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaRomanshPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-romanshPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiRomanshPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-romanshPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdRomanshPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-romanshPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsRomanshPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-romanshPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdRomanshPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-romanshPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfRomanshPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-romanshPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekRomanshPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-germanCount",
            Object = statisticDto.AuthoritiesCommissionsGermanCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-frenchCount",
            Object = statisticDto.AuthoritiesCommissionsFrenchCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-italianCount",
            Object = statisticDto.AuthoritiesCommissionsItalianCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-romanshCount",
            Object = statisticDto.AuthoritiesCommissionsRomanshCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-germanPercentage",
            Object = statisticDto.AuthoritiesCommissionsGermanPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-frenchPercentage",
            Object = statisticDto.AuthoritiesCommissionsFrenchPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-italianPercentage",
            Object = statisticDto.AuthoritiesCommissionsItalianPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-romanshPercentage",
            Object = statisticDto.AuthoritiesCommissionsRomanshPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-germanCount",
            Object = statisticDto.AdministrationCommissionsGermanCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-frenchCount",
            Object = statisticDto.AdministrationCommissionsFrenchCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-italianCount",
            Object = statisticDto.AdministrationCommissionsItalianCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-romanshCount",
            Object = statisticDto.AdministrationCommissionsRomanshCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-germanPercentage",
            Object = statisticDto.AdministrationCommissionsGermanPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-frenchPercentage",
            Object = statisticDto.AdministrationCommissionsFrenchPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-italianPercentage",
            Object = statisticDto.AdministrationCommissionsItalianPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-romanshPercentage",
            Object = statisticDto.AdministrationCommissionsRomanshPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-total-femaleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsTotalFemaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-total-maleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsTotalMaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-total-femalePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsTotalFemalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-total-malePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsTotalMalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-total-germanCount",
            Object = statisticDto.ExtraParliamentaryCommissionsTotalFrenchCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-total-frenchCount",
            Object = statisticDto.ExtraParliamentaryCommissionsTotalFrenchCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-total-italianCount",
            Object = statisticDto.ExtraParliamentaryCommissionsTotalItalianCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-total-romanshCount",
            Object = statisticDto.ExtraParliamentaryCommissionsTotalRomanshCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-total-germanPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsTotalGermanPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-total-frenchPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsTotalFrenchPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-total-italianPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsTotalItalianPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-total-romanshPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsTotalRomanshPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        // 2nd Group of committees (non extra parliamentary commissions)

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-count",
            Object = statisticDto.NonExtraParliamentaryCommissionsCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-count",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-count",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-count",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-count",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-count",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-count",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-count",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-count",
            Object = statisticDto.ManagementCommitteesCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-count",
            Object = statisticDto.FederalAgenciesCommitteesCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-{departmentEda}-count",
            Object = statisticDto.ManagementCommitteesEdaCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-{departmentEdi}-count",
            Object = statisticDto.ManagementCommitteesEdiCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-{departmentEjpd}-count",
            Object = statisticDto.ManagementCommitteesEjpdCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-{departmentVbs}-count",
            Object = statisticDto.ManagementCommitteesVbsCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-{departmentEfd}-count",
            Object = statisticDto.ManagementCommitteesEfdCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-{departmentWbf}-count",
            Object = statisticDto.ManagementCommitteesWbfCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-{departmentUvek}-count",
            Object = statisticDto.ManagementCommitteesUvekCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-{departmentEda}-count",
            Object = statisticDto.FederalAgenciesCommitteesEdaCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-{departmentEdi}-count",
            Object = statisticDto.FederalAgenciesCommitteesEdiCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-{departmentEjpd}-count",
            Object = statisticDto.FederalAgenciesCommitteesEjpdCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-{departmentVbs}-count",
            Object = statisticDto.FederalAgenciesCommitteesVbsCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-{departmentEfd}-count",
            Object = statisticDto.FederalAgenciesCommitteesEfdCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-{departmentWbf}-count",
            Object = statisticDto.FederalAgenciesCommitteesWbfCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-{departmentUvek}-count",
            Object = statisticDto.FederalAgenciesCommitteesUvekCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-femaleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaFemaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-femaleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiFemaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-femaleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdFemaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-femaleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsFemaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-femaleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdFemaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-femaleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfFemaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-femaleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekFemaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-maleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaMaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-maleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiMaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-maleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdMaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-maleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsMaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-maleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdMaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-maleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfMaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-maleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekMaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-femalePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaFemalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-femalePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiFemalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-femalePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdFemalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-femalePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsFemalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-femalePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdFemalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-femalePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfFemalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-femalePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekFemalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-malePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaMalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-malePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiMalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-malePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdMalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-malePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsMalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-malePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdMalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-malePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfMalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-malePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekMalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-femaleCount",
            Object = statisticDto.ManagementCommitteesFemaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-maleCount",
            Object = statisticDto.ManagementCommitteesMaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-femalePercentage",
            Object = statisticDto.ManagementCommitteesFemalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-malePercentage",
            Object = statisticDto.ManagementCommitteesMalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-femaleCount",
            Object = statisticDto.FederalAgenciesCommitteesFemaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-maleCount",
            Object = statisticDto.FederalAgenciesCommitteesMaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-femalePercentage",
            Object = statisticDto.FederalAgenciesCommitteesFemalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-malePercentage",
            Object = statisticDto.FederalAgenciesCommitteesMalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-germanCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaGermanCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-germanCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiGermanCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-germanCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdGermanCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-germanCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsGermanCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-germanCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdGermanCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-germanCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfGermanCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-germanCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekGermanCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-germanPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaGermanPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-germanPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiGermanPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-germanPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdGermanPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-germanPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsGermanPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-germanPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdGermanPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-germanPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfGermanPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-germanPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekGermanPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-frenchCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaFrenchCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-frenchCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiFrenchCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-frenchCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdFrenchCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-frenchCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsFrenchCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-frenchCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdFrenchCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-frenchCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfFrenchCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-frenchCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekFrenchCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-frenchPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaFrenchPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-frenchPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiFrenchPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-frenchPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdFrenchPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-frenchPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsFrenchPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-frenchPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdFrenchPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-frenchPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfFrenchPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-frenchPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekFrenchPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-italianCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaItalianCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-italianCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiItalianCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-italianCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdItalianCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-italianCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsItalianCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-italianCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdItalianCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-italianCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfItalianCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-italianCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekItalianCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-italianPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaItalianPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-italianPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiItalianPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-italianPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdItalianPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-italianPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsItalianPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-italianPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdItalianPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-italianPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfItalianPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-italianPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekItalianPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-romanshCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaRomanshCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-romanshCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiRomanshCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-romanshCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdRomanshCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-romanshCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsRomanshCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-romanshCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdRomanshCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-romanshCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfRomanshCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-romanshCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekRomanshCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-romanshPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaRomanshPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-romanshPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiRomanshPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-romanshPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdRomanshPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-romanshPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsRomanshPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-romanshPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdRomanshPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-romanshPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfRomanshPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-romanshPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekRomanshPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-germanCount",
            Object = statisticDto.ManagementCommitteesGermanCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-frenchCount",
            Object = statisticDto.ManagementCommitteesFrenchCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-italianCount",
            Object = statisticDto.ManagementCommitteesItalianCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-romanshCount",
            Object = statisticDto.ManagementCommitteesRomanshCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-germanPercentage",
            Object = statisticDto.ManagementCommitteesGermanPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-frenchPercentage",
            Object = statisticDto.ManagementCommitteesFrenchPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-italianPercentage",
            Object = statisticDto.ManagementCommitteesItalianPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-romanshPercentage",
            Object = statisticDto.ManagementCommitteesRomanshPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-germanCount",
            Object = statisticDto.FederalAgenciesCommitteesGermanCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-frenchCount",
            Object = statisticDto.FederalAgenciesCommitteesFrenchCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-italianCount",
            Object = statisticDto.FederalAgenciesCommitteesItalianCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-romanshCount",
            Object = statisticDto.FederalAgenciesCommitteesRomanshCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-germanPercentage",
            Object = statisticDto.FederalAgenciesCommitteesGermanPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-frenchPercentage",
            Object = statisticDto.FederalAgenciesCommitteesFrenchPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-italianPercentage",
            Object = statisticDto.FederalAgenciesCommitteesItalianPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-romanshPercentage",
            Object = statisticDto.FederalAgenciesCommitteesRomanshPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-total-femaleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsTotalFemaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-total-maleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsTotalMaleCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-total-femalePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsTotalFemalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-total-malePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsTotalMalePercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-total-germanCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsTotalFrenchCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-total-frenchCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsTotalFrenchCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-total-italianCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsTotalItalianCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-total-romanshCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsTotalRomanshCount.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeInt
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-total-germanPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsTotalGermanPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-total-frenchPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsTotalFrenchPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-total-italianPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsTotalItalianPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-total-romanshPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsTotalRomanshPercentage.ToString(),
            DataTypeUri = OgdExportConstants.DataTypeDecimal
        });

        return dataRow;
    }

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
                NameFr = "FR_Organisation",
                NameIt = "IT_Organisation",
                NameEn = "EN_Organisation",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:committeeCount",
            Object = statisticDto.CommitteeCount.ToString(),
            ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "Anzahl",
                NameFr = "FR_Anzahl",
                NameIt = "IT_Anzahl",
                NameEn = "EN_Anzahl",
                Type = OgdExportConstants.CubeMeasureDimension,
                ScaleType = OgdExportConstants.QudtRatioScale,
                NodeKind = OgdExportConstants.ShaclNodeKindLiteral
            }
        });

        return dataRow;
    }

}
