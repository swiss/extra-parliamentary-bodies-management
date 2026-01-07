using Bk.APG.Business.Models;

namespace Bk.APG.Business.Services;

public interface IMembershipTermCalculationService
{
    int CalculateCurrentTermInYears(IEnumerable<Membership> memberships);
    int CalculateEstimatedTermInYears(DateOnly beginDate, DateOnly endDate);
}
