using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
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

    public static ObservationDataRow ToCommitteeTypeStatisticObservation(CommitteeTypeDepartmentStatisticDto statisticDto)
    {
        var ogdNamespace = OgdExportConstants.NamespaceCommitteeTypeStatistic;

        var dataRow = new ObservationDataRow
        {
            // combined key committee and department
            KeyUri = $"{ogdNamespace}:{statisticDto.CommitteeTypeOgdId}-{statisticDto.DepartmentOgdId}"
        };

        dataRow.KeyDimensionLinks.Add(new KeyDimensionLink
        {
            Predicate = $"{ogdNamespace}:hasCommitteeType",
            Uri = $"{OgdExportConstants.NamespaceCommitteeType}:{statisticDto.CommitteeTypeOgdId}"
        });

        if (!string.IsNullOrWhiteSpace(statisticDto.DepartmentUri))
        {
            dataRow.KeyDimensionLinks.Add(
                new KeyDimensionLink { Predicate = $"{ogdNamespace}:hasDepartment", Uri = OgdExportConstants.CreateUriLinkForLdAdminCh(statisticDto.DepartmentUri) });
        }

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

        return dataRow;
    }
}
