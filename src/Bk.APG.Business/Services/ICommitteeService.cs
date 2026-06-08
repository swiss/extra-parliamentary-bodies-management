using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.CrossCutting;

namespace Bk.APG.Business.Services;

public interface ICommitteeService
{
    Task<PagedResultDto<CommitteeListDto>> GetCommitteeList(PagingParametersDto paging, CommitteeFilterParametersDto? filter, string? sort, SortDirection? sortDirection);
    Task<IEnumerable<CommitteeListDto>> GetCommitteeListForExport(ReportFilterParametersDto? filterParameters);
    Task<IEnumerable<Committee>> GetCommitteesForGeneralElection();
    Task<IEnumerable<Committee>> GetCommitteesWithRetiredMembers(GeneralElectionCommitteeExportFilterParametersDto? filter, List<Guid> electionTypeIds);
    Task<CommitteeDetailDto> GetCommitteeDetail(Guid id, bool ignoreGeneralElectionPart = false);
    Task<CommitteeUpdateDto> GetCommitteeForUpdate(Guid id);
    Task<CommitteeJustificationUpdateDto> GetCommitteeJustificationForUpdate(Guid id);
    Task<CommitteeDetailDto> UpdateCommittee(Guid id, CommitteeUpdateDto updateDto, bool checkAuthorization);
    Task<CommitteeDetailDto> UpdateCommitteeAfterGeneralElection(Guid id, CommitteeUpdateDto updateDto, List<MembershipCandidate> membershipCandidates);
    Task<CommitteeJustificationUpdateDto> UpdateCommitteeJustifications(Guid id, CommitteeJustificationUpdateDto updateDto);
    Task<CommitteeDetailDto> CreateCommittee(CommitteeCreateDto createDto);
    Task<IEnumerable<CommitteeDetailDto>> GetByDescription(string description);
    Task<CommitteeMembershipValidationResultDto> ValidateCommittee(Guid id, CommitteeMembershipValidationRequestDto validateDto);
    Task<CommitteeCreateDto> GetEmpty();
    Task<IEnumerable<CommitteeTypeStatisticDto>> GetCommitteeTypeStatistic();
    Task<IEnumerable<CommitteeTypeDepartmentStatisticDto>> GetCommitteeTypeDepartmentStatistic();
}
