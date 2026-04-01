using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Mapper;

public static class CommitteeTypeMapper
{
    public static CommitteeTypeListDto ToCommitteeTypeListDto(CommitteeType committeeType, CultureInfo cultureInfo)
    {
        ArgumentNullException.ThrowIfNull(committeeType);

        return new CommitteeTypeListDto
        {
            Id = committeeType.Id,
            Description = committeeType.GetDescription(cultureInfo),
            Text = committeeType.GetText(cultureInfo),
            FemaleThreshold = committeeType.FemaleThreshold,
            MaleThreshold = committeeType.MaleThreshold,
            GermanMinimalThreshold = committeeType.GermanMinimalThreshold,
            FrenchMinimalThreshold = committeeType.FrenchMinimalThreshold,
            ItalianMinimalThreshold = committeeType.ItalianMinimalThreshold,
            RomanshMinimalThreshold = committeeType.RomanshMinimalThreshold,
            GermanThresholdPercentage = committeeType.GermanThresholdPercentage,
            FrenchThresholdPercentage = committeeType.FrenchThresholdPercentage,
            ItalianThresholdPercentage = committeeType.ItalianThresholdPercentage,
            RomanshThresholdPercentage = committeeType.RomanshThresholdPercentage,
        };
    }

    public static CommitteeTypeUpdateDto ToCommitteeTypeUpdateDto(CommitteeType committeeType, CultureInfo cultureInfo)
    {
        ArgumentNullException.ThrowIfNull(committeeType);

        return new CommitteeTypeUpdateDto
        {
            Id = committeeType.Id,
            Text = committeeType.GetText(cultureInfo),
            FemaleThreshold = committeeType.FemaleThreshold,
            MaleThreshold = committeeType.MaleThreshold,
            GermanMinimalThreshold = committeeType.GermanMinimalThreshold,
            FrenchMinimalThreshold = committeeType.FrenchMinimalThreshold,
            ItalianMinimalThreshold = committeeType.ItalianMinimalThreshold,
            RomanshMinimalThreshold = committeeType.RomanshMinimalThreshold,
            GermanThresholdPercentage = committeeType.GermanThresholdPercentage,
            FrenchThresholdPercentage = committeeType.FrenchThresholdPercentage,
            ItalianThresholdPercentage = committeeType.ItalianThresholdPercentage,
            RomanshThresholdPercentage = committeeType.RomanshThresholdPercentage,
            RowVersion = committeeType.RowVersion
        };
    }
}
