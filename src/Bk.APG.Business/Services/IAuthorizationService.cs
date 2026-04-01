using Bk.APG.Business.Models;

namespace Bk.APG.Business.Services;

public interface IAuthorizationService
{
    bool IsAdmin { get; }
    bool IsSecretariat { get; }
    bool IsDepartment { get; }
    bool IsOffice { get; }
    bool IsObserver { get; }
    string GetCurrentUserName();
    Task<bool> IsCommitteeAssigned(Guid committeeId);
    Task<Department?> GetDepartment();
    Task<Office?> GetOffice();
    Task<IEnumerable<Committee>> LoadCommittees();
    Task<bool> HasAccessToCommittee(Committee committee);
    Task<EiamAssignment> GetCurrentEiamAssignment();
}
