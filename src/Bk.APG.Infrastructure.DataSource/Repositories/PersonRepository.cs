using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.CrossCutting;
using Bk.APG.CrossCutting.Exception;
using Bk.APG.Infrastructure.DataSource.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Infrastructure.DataSource.Repositories;

public class PersonRepository : IPersonRepository
{
    private readonly DataContext _dataContext;

    public PersonRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<PagedResult<Person>> GetAll(PagingParameters paging, PersonFilterParameters? filter, string? sort, SortDirection? sortDirection)
    {
        var query = _dataContext.Persons
            .Include(item => item.Language)
            .Include(item => item.Office)
            .Include(item => item.Council)
            .Include(item => item.Interests)
            .Include(item => item.Occupations)
            .Include(item => item.PrivateAddress)
                .ThenInclude(item => item!.Canton)
            .Include(item => item.OfficeAddress)
                .ThenInclude(item => item!.Canton)
            .Include(item => item.CorrespondenceAddress)
                .ThenInclude(item => item!.Canton)
            .Include(item => item.Memberships)
                .ThenInclude(item => item.ElectionType)
            .Include(item => item.Memberships)
                .ThenInclude(item => item.Committee)
                    .ThenInclude(item => item!.CommitteeType)
            .FilterPersons(filter)
            .AsSplitQuery()
            .AsNoTracking();

        var count = await query
            .CountAsync();

        var items = await query
            .SortPersons(sort ?? "surname", sortDirection.GetValueOrDefault(SortDirection.Desc))
            .Skip(paging.PageIndex * paging.PageSize)
            .Take(paging.PageSize)
            .ToListAsync();

        return new PagedResult<Person>
        {
            Index = paging.PageIndex,
            Total = count,
            Items = items
        };
    }

    public IEnumerable<Person> GetAll()
    {
        return _dataContext.Persons
            .Include(item => item.Occupations)
            .Include(item => item.Office)
                .ThenInclude(item => item!.Department)
            .AsEnumerable();
    }

    public async Task<IEnumerable<Person>> GetByName(string name)
    {
        var filters = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var query = GetPersons()
            .AsNoTracking();

        foreach (var filter in filters)
        {
            var likeFilter = $"%{filter}%";

            query = query.Where(y =>
                EF.Functions.ILike(y.GivenName, likeFilter)
                || EF.Functions.ILike(y.Surname, likeFilter)
                || EF.Functions.ILike(y.BirthYear.ToString(), likeFilter));
        }

        var persons = await query
            .OrderBy(y => y.Surname)
            .ThenBy(y => y.GivenName)
            .ThenBy(y => y.BirthYear)
            .ToListAsync();

        return persons;
    }

    public async Task<Person> GetById(Guid id)
    {
        var person = await GetPersons()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (person is null)
        {
            throw new EntityNotFoundException($"Person Id={id} not found");
        }

        return person;
    }

    public async Task<Person> GetByIdForUpdate(Guid id, uint? updateDtoRowVersion = null)
    {
        var person = await GetPersons()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (person is null)
        {
            throw new EntityNotFoundException($"Person Id={id} not found");
        }

        if (updateDtoRowVersion.HasValue && updateDtoRowVersion != person.RowVersion)
        {
            _dataContext.Entry(person).Property(x => x.RowVersion).OriginalValue = updateDtoRowVersion.Value;
        }

        return person;
    }

    public async Task<Person> Create(Person person)
    {
        var entry = await _dataContext.Persons.AddAsync(person);
        await _dataContext.SaveChangesAsync();

        return entry.Entity;
    }

    public async Task CommitChanges()
    {
        await _dataContext.SaveChangesAsync();
    }

    public void Delete(Person person)
    {
        _dataContext.Persons.Remove(person);
    }

    public void CreateForMigration(Person person)
    {
        _dataContext.Persons.Add(person);
    }

    public async Task<IEnumerable<Person>> GetAllForDuplicateCheck()
    {
        return await _dataContext.Persons
            .AsNoTracking()
            .Include(p => p.Language)
            .Include(p => p.Gender)
            .Include(p => p.CorrespondenceAddress)
            .Include(p => p.CorrespondenceLanguage)
            .ToListAsync();
    }

    public async Task<IEnumerable<Person>> GetPersonsByBirthYear(int birthYear, int range = 0)
    {
        var persons = await GetPersons()
            .AsNoTracking()
            .Where(p => Math.Abs(p.BirthYear - birthYear) <= range)
            .ToListAsync();

        return persons;
    }

    public async Task<Person[]> GetPersonsForExport(DateOnly startDate)
    {
        var persons = await GetPersons()
            .Where(p => p.Memberships.Any(m => m.BeginDate <= startDate && m.EndDate >= startDate))
            .OrderBy(p => p.Surname)
            .ThenBy(p => p.GivenName)
            .Include(p => p.Memberships)
            .Include(p => p.CorrespondenceAddress)
            .Include(p => p.Language)
            .Include(p => p.Gender)
            .AsNoTracking()
            .AsSplitQuery()
            .ToArrayAsync();

        return persons;
    }

    private IQueryable<Person> GetPersons()
    {
        return _dataContext.Persons
            .Include(item => item.Language)
            .Include(item => item.Office)
            .Include(item => item.Council)
            .Include(item => item.PrivateAddress)
                .ThenInclude(item => item!.Canton)
            .Include(item => item.OfficeAddress)
                .ThenInclude(item => item!.Canton)
            .Include(item => item.CorrespondenceAddress)
               .ThenInclude(item => item!.Canton)
            .Include(item => item.Memberships)
              .ThenInclude(item => item!.Committee)
            .Include(item => item.Memberships)
               .ThenInclude(item => item.ElectionType)
            .Include(item => item.Memberships)
                .ThenInclude(item => item.Function)
            .Include(item => item.Interests)
                .ThenInclude(item => item.InterestCommittee)
            .Include(item => item.Interests)
                .ThenInclude(item => item.InterestFunction)
            .Include(item => item.Interests)
                .ThenInclude(item => item.InterestLegalForm)
            .Include(item => item.Interests)
                .ThenInclude(item => item.LegalForm)
            .Include(item => item.Gender)
            .Include(item => item.CorrespondenceLanguage)
            .Include(item => item.Salutation)
            .Include(item => item.LegislaturePeriods)
            .Include(item => item.Occupations)
            .AsSplitQuery();
    }
}
