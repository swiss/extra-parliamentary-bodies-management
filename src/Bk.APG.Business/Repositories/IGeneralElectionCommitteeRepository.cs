using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.CrossCutting;

namespace Bk.APG.Business.Repositories;

public interface IGeneralElectionCommitteeRepository
{
    Task<GeneralElectionCommittee> GetById(Guid id);
    Task<GeneralElectionCommittee> GetByIdForUpdate(Guid id, uint? updateDtoRowVersion = null);
    Task<PagedResult<GeneralElectionCommittee>> GetAll(PagingParameters paging, GeneralElectionCommitteeFilterParameters filter, string? sort, SortDirection? sortDirection);
    Task<IEnumerable<GeneralElectionCommittee>> GetByFilterForReport(ReportFilterParametersDto filterDto, Guid departmentId, Guid officeId, Guid committeeId);
    Task CommitChanges();
    Task<GeneralElectionCommittee> Create(GeneralElectionCommittee committee);
    Task<IEnumerable<GeneralElectionCommittee>> GetByDepartmentId(Guid departmentId);
    Task<IEnumerable<GeneralElectionCommittee>> GetByOfficeId(Guid officeId);
    Task<GeneralElectionCommittee> GetByCommitteeId(Guid committeeId);
    Task DeleteAll();
    Task<GeneralElectionCommittee> GetByCommitteeIdForUpdate(Guid committeeId, uint? updateDtoRowVersion = null);
    Task<GeneralElectionCommittee> GetForCandidateListExport(Guid committeeId);
}
