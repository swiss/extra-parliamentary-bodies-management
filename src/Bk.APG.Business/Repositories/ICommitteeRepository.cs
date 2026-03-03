using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.CrossCutting;

namespace Bk.APG.Business.Repositories;

public interface ICommitteeRepository
{
    Task<Committee> GetById(Guid id);
    void CreateForMigration(Committee committee);
    Task<PagedResult<Committee>> GetAll(PagingParameters pagingParameters, CommitteeFilterParameters? filterParameters, string? sort, SortDirection? sortDirection);
    Task<IEnumerable<Committee>> GetAllForExport(Guid departmentId, Guid officeId, Guid committeeId, CommitteeExportFilterParametersDto? filter);
    Task<IEnumerable<Committee>> GetAllForGeneralElection(Guid departmentId, Guid officeId, Guid committeeId);
    Task<IEnumerable<Committee>> GetAllForGeneralElectionWithActiveMembers(Guid departmentId, Guid officeId, Guid committeeId);
    Task<IEnumerable<Committee>> GetByFilterForReport(ReportFilterParametersDto filterDto, Guid departmentId, Guid officeId, Guid committeeId);
    Task<Committee> GetByIdForUpdate(Guid id, uint? updateDtoRowVersion = null);
    IEnumerable<Committee> GetAll();
    Task CommitChanges();
    Task<Committee> Create(Committee committee);
    Task<IEnumerable<Committee>> GetByDescription(string description);
    Task<IEnumerable<Committee>> GetForGeneralElectionByDepartmentId(Guid departmentId);
    Task<IEnumerable<Committee>> GetForGeneralElectionByOfficeId(Guid officeId);
    Task<Committee[]> GetCommitteesForExport(DateOnly startDate, Guid departmentId, Guid officeId, Guid committeeId);
    Task<Committee[]> GetCommitteesWithInterestsForExport(DateOnly startDate, Guid departmentId, Guid officeId, Guid committeeId);
    Task<Committee[]> GetCommitteesWithContactPointsForExport(DateOnly startDate, Guid departmentId, Guid officeId, Guid committeeId);
    Task<Committee[]> GetCommitteesForRegionExport(DateOnly startDate, Guid departmentId, Guid officeId, Guid committeeId);
    Task<Committee[]> GetCommitteeDataForStatistics();
    Task<IEnumerable<Committee>> GetForOgdExport();
}
