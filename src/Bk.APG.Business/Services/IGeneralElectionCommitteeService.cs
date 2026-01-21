using Bk.APG.Business.Dtos;
using Bk.APG.CrossCutting;

namespace Bk.APG.Business.Services;

public interface IGeneralElectionCommitteeService
{
    Task<GeneralElectionCommitteeDetailDto> GetGeneralElectionCommittee(Guid committeeId);
    Task<GeneralElectionCommitteeJustificationUpdateDto> GetGeneralElectionCommitteeJustificationForUpdate(Guid id);
    Task<PagedResultDto<GeneralElectionCommitteeListDto>> GetGeneralElectionCommitteeList(PagingParametersDto paging, GeneralElectionCommitteeFilterParametersDto? filter, string? sort, SortDirection? sortDirection);
    Task<GeneralElectionCommitteeUpdateDto> GetGeneralElectionCommitteeForUpdate(Guid committeeId);
    Task<GeneralElectionCommitteeDetailDto> UpdateGeneralElectionCommittee(Guid committeeId, GeneralElectionCommitteeUpdateDto updateDto);
    Task<GeneralElectionCommitteeJustificationUpdateDto> UpdateGeneralElectionCommitteeJustifications(Guid committeeId, GeneralElectionCommitteeJustificationUpdateDto updateDto);
    Task<GeneralElectionCommitteeUpdateDto> UpdateGeneralElectionCommitteeVacancies(Guid id, int vacancies);
    Task<(string fileName, Stream content)> GenerateCandidateListExport(Guid id, IEnumerable<Guid> membershipCandidateIds);
}
