using Bk.APG.Business.Models;

namespace Bk.APG.Business.Services;

public static class MembershipTermCalculator
{
    public static int CalculateCurrentTermInYears(IEnumerable<Membership> memberships)
    {
        var todayDate = DateOnly.FromDateTime(DateTime.Today);

        return memberships
            .Sum(x =>
            {
                var endDate = x.EndDate > todayDate
                    ? todayDate
                    : AdjustEndDate(x.EndDate);
                return CalculateTermsInYears(x.BeginDate, endDate);
            });
    }

    private static int CalculateTermsInYears(DateOnly beginDate, DateOnly endDate)
    {
        var numOfYears = endDate.Year - beginDate.Year;

        // If the end month is earlier than the beginning month, it decrements the year count (e.g., from Jan 2020 to Nov 2023 is 3 years, not 4).
        if (endDate.Month < beginDate.Month)
        {
            --numOfYears;
        }

        // If the months are the same but the end day is earlier than the beginning day, it also decrements the year count (e.g., from Jan 15 to Jan 10 hasn't completed a full year yet).
        if (endDate.Month == beginDate.Month && endDate.Day < beginDate.Day)
        {
            --numOfYears;
        }

        return numOfYears < 0
            ? 0
            : numOfYears;
    }

    private static DateOnly AdjustEndDate(DateOnly endDate)
    {
        // If the end date is 31st December, we consider the membership to have ended on 1st January of the next year.
        return endDate is { Month: 12, Day: 31 } ? endDate.AddDays(1) : endDate;
    }

    public static int CalculateEstimatedTermInYears(DateOnly beginDate, DateOnly endDate)
    {
        var isDecember31 = endDate is { Month: 12, Day: 31 };
        var numOfYears = endDate.Year - beginDate.Year;

        if (isDecember31)
        {
            // For 31.12 dates: if begin date is 01.01, add 1 (serving through entire calendar years)
            if (beginDate is { Month: 1, Day: 1 })
            {
                ++numOfYears;
            }
        }
        else
        {
            // For non-31.12 dates: check if end date is on or after anniversary date
            var anniversaryDate = beginDate.AddYears(numOfYears);
            if (endDate >= anniversaryDate)
            {
                ++numOfYears;
            }
        }

        return numOfYears < 0 ? 0 : numOfYears;
    }
}
