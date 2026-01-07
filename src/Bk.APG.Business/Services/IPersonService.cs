using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.CrossCutting;

namespace Bk.APG.Business.Services;

public interface IPersonService
{
    Task<PagedResultDto<PersonListDto>> GetPersonList(PagingParametersDto paging, PersonFilterParametersDto? filterParametersDto, string? sort, SortDirection? sortDirection);
    Task<PersonDetailDto> GetPersonDetail(Guid id);
    Task<PersonUpdateDto> UpdatePerson(Guid id, PersonUpdateDto updateDto);
    Task<PersonUpdateDto> GetPersonForUpdate(Guid id);
    PersonCreateDto GetEmpty();
    Task<PersonDetailDto> CreatePerson(PersonCreateDto updateDto);
    Task<IEnumerable<PersonDetailDto>?> GetSimilarPersons(string surname, string givenName, int birthYear, int birthYearRange);
    Task<IEnumerable<PersonDetailDto>?> GetByName(string name);
    Task<CandidateListDuplicateCheckResultDto> GetDuplicatePersonForGeneralElection(MembershipCandidate membershipCandidate);
    Task<PersonDetailDto> CreatePersonInGeneralElection(MembershipCandidate membershipCandidate);
    bool ShouldMaskAddress(Person person);
    Task DeletePerson(Guid id);
}
