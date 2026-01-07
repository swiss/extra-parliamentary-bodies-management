using Bk.APG.Business.Models;
using Bk.APG.CrossCutting;

namespace Bk.APG.Business.Repositories;

public interface IPersonRepository
{
    Task<PagedResult<Person>> GetAll(PagingParameters paging, PersonFilterParameters? filter, string? sort, SortDirection? sortDirection);
    IEnumerable<Person> GetAll();
    Task<Person> GetById(Guid id);
    Task<Person> GetByIdForUpdate(Guid id, uint? updateDtoRowVersion = null);
    Task<Person> Create(Person person);
    Task CommitChanges();
    void Delete(Person person);
    void CreateForMigration(Person person);
    Task<IEnumerable<Person>> GetPersonsByBirthYear(int birthYear, int range = 0);
    Task<IEnumerable<Person>> GetByName(string name);
    Task<Person[]> GetPersonsForExport(DateOnly startDate, Guid departmentId, Guid officeId, Guid committeeId);
    Task<IEnumerable<Person>> GetAllForDuplicateCheck();
}
