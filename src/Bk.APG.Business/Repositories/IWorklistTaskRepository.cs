using Bk.APG.Business.Models;
using Bk.APG.CrossCutting;

namespace Bk.APG.Business.Repositories;

public interface IWorklistTaskRepository
{
    Task<PagedResult<WorklistTask>> GetAll(PagingParameters paging, WorklistFilterParameters? filter, string? sort, SortDirection? sortDirection);
    Task<WorklistTask> GetByIdForUpdate(Guid id, uint? updateDtoRowVersion = null);
    Task<WorklistTask> GetByIdForForward(Guid id);
    Task<WorklistTask> Create(WorklistTask worklistTask);
    Task CreateRange(IEnumerable<WorklistTask> worklistTasks);
    Task<IEnumerable<WorklistTask>> GetByTermOfOfficeDateId(Guid id);
    Task<IEnumerable<WorklistTask>> GetByPersonOrMemberships(Guid personId, IEnumerable<Guid> membershipIds, IEnumerable<Guid> membershipCandidateIds);
    void DeleteRange(IEnumerable<WorklistTask> worklistTasks);
    Task Update(WorklistTask worklistTask);
    Task<IEnumerable<WorklistTask>> GetAllByGeneralElectionCommitteeId(Guid generalElectionCommitteeId);
    Task CommitChanges();
    Task<List<WorklistTask>> GetByWorklistTaskTypeId(Guid worklistTaskTypeId);
    Task<List<WorklistTask>> GetAllByPersonId(Guid personId);
}
