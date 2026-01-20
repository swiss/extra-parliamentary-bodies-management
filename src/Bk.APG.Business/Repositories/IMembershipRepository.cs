using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Repositories;

public interface IMembershipRepository
{
    void CreateForMigration(Membership membership);
    Task<IEnumerable<Membership>> GetAllByPersonId(Guid personId);
    Task<IEnumerable<Membership>> GetAllByCommitteeId(Guid committeeId);
    IEnumerable<Membership> GetAllActiveForOgdExport();
    Task<Membership> GetById(Guid id);
    Task<Membership> Create(Membership membership);
    Task<Membership> GetByIdForUpdate(Guid id, uint? updateDtoRowVersion = null);
    Task<IEnumerable<Membership>> GetAllActiveMemberships();
    Task<IEnumerable<Membership>> GetAllActiveMembershipsForCommittee(Guid committeeId);
    Task<IEnumerable<Membership>> GetAllMembershipsForCommitteeAndPerson(Guid committeeId, Guid personId);
    IEnumerable<MembershipFunctionStatisticDto> GetMembershipFunctionsForStatistics();
    Task CommitChanges();
    void Delete(Membership membership);
    void DeleteRange(IEnumerable<Membership> memberships);
}
