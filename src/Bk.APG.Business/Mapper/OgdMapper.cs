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
            Uri = $"committee:{statisticDto.CommitteeOgdId}"
        });

        dataRow.KeyDimensionLinks.Add(new KeyDimensionLink
        {
            Predicate = $"{ogdNamespace}:hasFunction",
            Uri = $"function:{ogdId}"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:functionCount",
            Object = statisticDto.FunctionCount.ToString()
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
            Uri = $"committee:{statisticDto.CommitteeOgdId}"
        });

        dataRow.KeyDimensionLinks.Add(new KeyDimensionLink
        {
            Predicate = $"{ogdNamespace}:hasCanton",
            Uri = $"canton:{statisticDto.CantonOgdId}"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:cantonCount",
            Object = statisticDto.CantonCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });
        return dataRow;
    }

    public static ObservationDataRow ToGenderLanguageStatisticObservation(MembershipGenderLanguageStatisticDto statisticDto)
    {
        var ogdNamespace = OgdExportConstants.NamespaceCommitteeGenderLanguageStatistic;

        var dataRow = new ObservationDataRow
        {
            KeyUri = $"{ogdNamespace}:{statisticDto.CommitteeOgdId}"
        };

        dataRow.KeyDimensionLinks.Add(new KeyDimensionLink
        {
            Predicate = $"{ogdNamespace}:hasCommittee",
            Uri = $"committee:{statisticDto.CommitteeOgdId}"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:femaleCount",
            Object = statisticDto.FemaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:femalePercentage",
            Object = statisticDto.FemalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:maleCount",
            Object = statisticDto.MaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:malePercentage",
            Object = statisticDto.MalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:germanCount",
            Object = statisticDto.GermanCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:germanPercentage",
            Object = statisticDto.GermanPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:frenchCount",
            Object = statisticDto.FrenchCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:frenchPercentage",
            Object = statisticDto.FrenchPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:italianCount",
            Object = statisticDto.ItalianCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:italianPercentage",
            Object = statisticDto.ItalianPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:romanshCount",
            Object = statisticDto.RomanshCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:romanshPercentage",
            Object = statisticDto.RomanshPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:federalDutyCount",
            Object = statisticDto.FederalDutyCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:federalAssemblyCount",
            Object = statisticDto.FederalAssemblyCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:over40Count",
            Object = statisticDto.Over40Count.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:over40Percentage",
            Object = statisticDto.Over40Percentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:underOr40Count",
            Object = statisticDto.UnderOr40Count.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:underOr40Percentage",
            Object = statisticDto.UnderOr40Percentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
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
            Uri = $"committee:{statisticDto.CommitteeOgdId}"
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
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:femaleCount",
            Object = statisticDto.FemaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });
        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:femalePercentage",
            Object = statisticDto.FemalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:maleCount",
            Object = statisticDto.MaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:malePercentage",
            Object = statisticDto.MalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:germanCount",
            Object = statisticDto.GermanCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:germanPercentage",
            Object = statisticDto.GermanPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:frenchCount",
            Object = statisticDto.FrenchCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:frenchPercentage",
            Object = statisticDto.FrenchPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:italianCount",
            Object = statisticDto.ItalianCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:italianPercentage",
            Object = statisticDto.ItalianPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:romanshCount",
            Object = statisticDto.RomanshCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:romanshPercentage",
            Object = statisticDto.RomanshPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:federalDutyCount",
            Object = statisticDto.FederalDutyCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:federalAssemblyCount",
            Object = statisticDto.FederalAssemblyCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:over40Count",
            Object = statisticDto.Over40Count.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:over40Percentage",
            Object = statisticDto.Over40Percentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:underOr40Count",
            Object = statisticDto.UnderOr40Count.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:underOr40Percentage",
            Object = statisticDto.UnderOr40Percentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
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
            // combined key committee and department
            // KeyUri = $"{ogdNamespace}:{statisticDto.CommitteeTypeOgdId}-{statisticDto.DepartmentOgdId}"
            KeyUri = $"{ogdNamespace}:{1}-{1}"
        };

        //dataRow.KeyDimensionLinks.Add(new KeyDimensionLink
        //{
        //    Predicate = $"{ogdNamespace}:hasCommitteeType",
        //    Uri = $"{OgdExportConstants.NamespaceCommitteeType}:{statisticDto.CommitteeTypeOgdId}"
        //});

        //if (!string.IsNullOrWhiteSpace(statisticDto.DepartmentUri))
        //{
        //    dataRow.KeyDimensionLinks.Add(
        //        new KeyDimensionLink { Predicate = $"{ogdNamespace}:hasDepartment", Uri = OgdExportConstants.CreateUriLinkForLdAdminCh(statisticDto.DepartmentUri) });
        //}



        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-count",
            Object = statisticDto.ExtraParliamentaryCommissionsCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-count",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-count",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-count",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-count",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-count",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-count",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-count",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-count",
            Object = statisticDto.AuthoritiesCommissionsCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-count",
            Object = statisticDto.AdministrationCommissionsCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-{departmentEda}-count",
            Object = statisticDto.AuthoritiesCommissionsEdaCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-{departmentEdi}-count",
            Object = statisticDto.AuthoritiesCommissionsEdiCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-{departmentEjpd}-count",
            Object = statisticDto.AuthoritiesCommissionsEjpdCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-{departmentVbs}-count",
            Object = statisticDto.AuthoritiesCommissionsVbsCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-{departmentEfd}-count",
            Object = statisticDto.AuthoritiesCommissionsEfdCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-{departmentWbf}-count",
            Object = statisticDto.AuthoritiesCommissionsWbfCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-{departmentUvek}-count",
            Object = statisticDto.AuthoritiesCommissionsUvekCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-{departmentEda}-count",
            Object = statisticDto.AdministrationCommissionsEdaCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-{departmentEdi}-count",
            Object = statisticDto.AdministrationCommissionsEdiCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-{departmentEjpd}-count",
            Object = statisticDto.AdministrationCommissionsEjpdCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-{departmentVbs}-count",
            Object = statisticDto.AdministrationCommissionsVbsCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-{departmentEfd}-count",
            Object = statisticDto.AdministrationCommissionsEfdCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-{departmentWbf}-count",
            Object = statisticDto.AdministrationCommissionsWbfCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-{departmentUvek}-count",
            Object = statisticDto.AdministrationCommissionsUvekCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-femaleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaFemaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-femaleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiFemaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-femaleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdFemaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-femaleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsFemaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-femaleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdFemaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-femaleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfFemaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-femaleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekFemaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-maleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaMaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-maleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiMaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-maleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdMaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-maleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsMaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-maleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdMaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-maleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfMaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-maleCount",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekMaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-femalePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaFemalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-femalePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiFemalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-femalePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdFemalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-femalePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsFemalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-femalePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdFemalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-femalePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfFemalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-femalePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekFemalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-malePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaMalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-malePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiMalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-malePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdMalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-malePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsMalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-malePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdMalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-malePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfMalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-malePercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekMalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-femaleCount",
            Object = statisticDto.AuthoritiesCommissionsFemaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-maleCount",
            Object = statisticDto.AuthoritiesCommissionsMaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-femalePercentage",
            Object = statisticDto.AuthoritiesCommissionsFemalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-malePercentage",
            Object = statisticDto.AuthoritiesCommissionsMalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-femaleCount",
            Object = statisticDto.AdministrationCommissionsFemaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-maleCount",
            Object = statisticDto.AdministrationCommissionsMaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-femalePercentage",
            Object = statisticDto.AdministrationCommissionsFemalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-malePercentage",
            Object = statisticDto.AdministrationCommissionsMalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-germanCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaGermanCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-germanCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiGermanCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-germanCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdGermanCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-germanCount",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsGermanCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-germanCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdGermanCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-germanCount",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfGermanCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-germanCount",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekGermanCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-germanPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaGermanPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-germanPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiGermanPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-germanPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdGermanPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-germanPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsGermanPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-germanPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdGermanPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-germanPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfGermanPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-germanPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekGermanPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-frenchCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaFrenchCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-frenchCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiFrenchCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-frenchCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdFrenchCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-frenchCount",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsFrenchCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-frenchCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdFrenchCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-frenchCount",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfFrenchCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-frenchCount",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekFrenchCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-frenchPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaFrenchPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-frenchPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiFrenchPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-frenchPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdFrenchPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-frenchPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsFrenchPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-frenchPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdFrenchPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-frenchPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfFrenchPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-frenchPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekFrenchPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-italianCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaItalianCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-italianCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiItalianCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-italianCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdItalianCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-italianCount",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsItalianCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-italianCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdItalianCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-italianCount",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfItalianCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-italianCount",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekItalianCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-italianPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaItalianPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-italianPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiItalianPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-italianPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdItalianPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-italianPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsItalianPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-italianPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdItalianPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-italianPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfItalianPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-italianPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekItalianPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-romanshCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaRomanshCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-romanshCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiRomanshCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-romanshCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdRomanshCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-romanshCount",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsRomanshCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-romanshCount",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdRomanshCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-romanshCount",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfRomanshCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-romanshCount",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekRomanshCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEda}-romanshPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdaRomanshPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEdi}-romanshPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEdiRomanshPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEjpd}-romanshPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEjpdRomanshPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentVbs}-romanshPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsVbsRomanshPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentEfd}-romanshPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsEfdRomanshPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentWbf}-romanshPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsWbfRomanshPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{apkNamespace}-{departmentUvek}-romanshPercentage",
            Object = statisticDto.ExtraParliamentaryCommissionsUvekRomanshPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-germanCount",
            Object = statisticDto.AuthoritiesCommissionsGermanCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-frenchCount",
            Object = statisticDto.AuthoritiesCommissionsFrenchCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-italianCount",
            Object = statisticDto.AuthoritiesCommissionsItalianCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-romanshCount",
            Object = statisticDto.AuthoritiesCommissionsRomanshCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-germanPercentage",
            Object = statisticDto.AuthoritiesCommissionsGermanPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-frenchPercentage",
            Object = statisticDto.AuthoritiesCommissionsFrenchPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-italianPercentage",
            Object = statisticDto.AuthoritiesCommissionsItalianPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{authoritiesCommissionNamespace}-romanshPercentage",
            Object = statisticDto.AuthoritiesCommissionsRomanshPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-germanCount",
            Object = statisticDto.AdministrationCommissionsGermanCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-frenchCount",
            Object = statisticDto.AdministrationCommissionsFrenchCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-italianCount",
            Object = statisticDto.AdministrationCommissionsItalianCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-romanshCount",
            Object = statisticDto.AdministrationCommissionsRomanshCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-germanPercentage",
            Object = statisticDto.AdministrationCommissionsGermanPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-frenchPercentage",
            Object = statisticDto.AdministrationCommissionsFrenchPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-italianPercentage",
            Object = statisticDto.AdministrationCommissionsItalianPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{administrationCommissionNamespace}-romanshPercentage",
            Object = statisticDto.AdministrationCommissionsRomanshPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        // 2. Teil

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-count",
            Object = statisticDto.NonExtraParliamentaryCommissionsCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-count",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-count",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-count",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-count",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-count",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-count",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-count",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-count",
            Object = statisticDto.ManagementCommitteesCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-count",
            Object = statisticDto.FederalAgenciesCommitteesCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-{departmentEda}-count",
            Object = statisticDto.ManagementCommitteesEdaCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-{departmentEdi}-count",
            Object = statisticDto.ManagementCommitteesEdiCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-{departmentEjpd}-count",
            Object = statisticDto.ManagementCommitteesEjpdCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-{departmentVbs}-count",
            Object = statisticDto.ManagementCommitteesVbsCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-{departmentEfd}-count",
            Object = statisticDto.ManagementCommitteesEfdCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-{departmentWbf}-count",
            Object = statisticDto.ManagementCommitteesWbfCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-{departmentUvek}-count",
            Object = statisticDto.ManagementCommitteesUvekCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });



        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-{departmentEda}-count",
            Object = statisticDto.FederalAgenciesCommitteesEdaCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-{departmentEdi}-count",
            Object = statisticDto.FederalAgenciesCommitteesEdiCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-{departmentEjpd}-count",
            Object = statisticDto.FederalAgenciesCommitteesEjpdCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-{departmentVbs}-count",
            Object = statisticDto.FederalAgenciesCommitteesVbsCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-{departmentEfd}-count",
            Object = statisticDto.FederalAgenciesCommitteesEfdCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-{departmentWbf}-count",
            Object = statisticDto.FederalAgenciesCommitteesWbfCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-{departmentUvek}-count",
            Object = statisticDto.FederalAgenciesCommitteesUvekCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-femaleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaFemaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-femaleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiFemaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-femaleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdFemaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-femaleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsFemaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-femaleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdFemaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-femaleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfFemaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-femaleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekFemaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-maleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaMaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-maleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiMaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-maleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdMaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-maleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsMaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-maleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdMaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-maleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfMaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-maleCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekMaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-femalePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaFemalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-femalePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiFemalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-femalePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdFemalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-femalePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsFemalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-femalePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdFemalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-femalePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfFemalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-femalePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekFemalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-malePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaMalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-malePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiMalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-malePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdMalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-malePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsMalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-malePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdMalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-malePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfMalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-malePercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekMalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-femaleCount",
            Object = statisticDto.ManagementCommitteesFemaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-maleCount",
            Object = statisticDto.ManagementCommitteesMaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-femalePercentage",
            Object = statisticDto.ManagementCommitteesFemalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-malePercentage",
            Object = statisticDto.ManagementCommitteesMalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-femaleCount",
            Object = statisticDto.FederalAgenciesCommitteesFemaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-maleCount",
            Object = statisticDto.FederalAgenciesCommitteesMaleCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-femalePercentage",
            Object = statisticDto.FederalAgenciesCommitteesFemalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-malePercentage",
            Object = statisticDto.FederalAgenciesCommitteesMalePercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-germanCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaGermanCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-germanCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiGermanCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-germanCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdGermanCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-germanCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsGermanCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-germanCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdGermanCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-germanCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfGermanCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-germanCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekGermanCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-germanPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaGermanPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-germanPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiGermanPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-germanPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdGermanPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-germanPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsGermanPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-germanPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdGermanPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-germanPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfGermanPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-germanPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekGermanPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-frenchCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaFrenchCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-frenchCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiFrenchCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-frenchCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdFrenchCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-frenchCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsFrenchCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-frenchCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdFrenchCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-frenchCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfFrenchCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-frenchCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekFrenchCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-frenchPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaFrenchPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-frenchPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiFrenchPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-frenchPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdFrenchPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-frenchPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsFrenchPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-frenchPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdFrenchPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-frenchPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfFrenchPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-frenchPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekFrenchPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-italianCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaItalianCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-italianCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiItalianCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-italianCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdItalianCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-italianCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsItalianCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-italianCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdItalianCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-italianCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfItalianCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-italianCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekItalianCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-italianPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaItalianPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-italianPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiItalianPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-italianPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdItalianPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-italianPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsItalianPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-italianPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdItalianPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-italianPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfItalianPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-italianPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekItalianPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-romanshCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaRomanshCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-romanshCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiRomanshCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-romanshCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdRomanshCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-romanshCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsRomanshCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-romanshCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdRomanshCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-romanshCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfRomanshCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-romanshCount",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekRomanshCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEda}-romanshPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdaRomanshPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEdi}-romanshPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEdiRomanshPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEjpd}-romanshPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEjpdRomanshPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });


        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentVbs}-romanshPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsVbsRomanshPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentEfd}-romanshPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsEfdRomanshPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentWbf}-romanshPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsWbfRomanshPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{nonApkNamespace}-{departmentUvek}-romanshPercentage",
            Object = statisticDto.NonExtraParliamentaryCommissionsUvekRomanshPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-germanCount",
            Object = statisticDto.ManagementCommitteesGermanCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-frenchCount",
            Object = statisticDto.ManagementCommitteesFrenchCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-italianCount",
            Object = statisticDto.ManagementCommitteesItalianCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-romanshCount",
            Object = statisticDto.ManagementCommitteesRomanshCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-germanPercentage",
            Object = statisticDto.ManagementCommitteesGermanPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-frenchPercentage",
            Object = statisticDto.ManagementCommitteesFrenchPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-italianPercentage",
            Object = statisticDto.ManagementCommitteesItalianPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{managementCommissionNamespace}-romanshPercentage",
            Object = statisticDto.ManagementCommitteesRomanshPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-germanCount",
            Object = statisticDto.FederalAgenciesCommitteesGermanCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-frenchCount",
            Object = statisticDto.FederalAgenciesCommitteesFrenchCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-italianCount",
            Object = statisticDto.FederalAgenciesCommitteesItalianCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-romanshCount",
            Object = statisticDto.FederalAgenciesCommitteesRomanshCount.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#int"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-germanPercentage",
            Object = statisticDto.FederalAgenciesCommitteesGermanPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-frenchPercentage",
            Object = statisticDto.FederalAgenciesCommitteesFrenchPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-italianPercentage",
            Object = statisticDto.FederalAgenciesCommitteesItalianPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        dataRow.Values.Add(new DimensionValue
        {
            Predicate = $"{ogdNamespace}:{federalAgenciesCommissionNamespace}-romanshPercentage",
            Object = statisticDto.FederalAgenciesCommitteesRomanshPercentage.ToString(),
            DataTypeUri = "http://www.w3.org/2001/XMLSchema#decimal"
        });

        return dataRow;
    }
}
