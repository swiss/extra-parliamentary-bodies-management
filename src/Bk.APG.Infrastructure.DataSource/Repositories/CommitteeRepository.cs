using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting;
using Bk.APG.CrossCutting.Exception;
using Bk.APG.Infrastructure.DataSource.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Infrastructure.DataSource.Repositories;

public class CommitteeRepository : ICommitteeRepository
{
    private readonly DataContext _dataContext;
    private readonly ICultureService _cultureService;

    public CommitteeRepository(DataContext dataContext, ICultureService cultureService)
    {
        _dataContext = dataContext;
        _cultureService = cultureService;
    }

    public async Task<Committee> GetById(Guid id)
    {
        var committee = await GetCommittees().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        if (committee is null)
        {
            throw new EntityNotFoundException($"Committee Id={id} not found");
        }

        return committee;
    }

    public async Task<IEnumerable<Committee>> GetForGeneralElectionByDepartmentId(Guid departmentId)
    {
        var committees = await _dataContext.Committees
            .Include(x => x.GeneralElectionCommittees)
                .ThenInclude(x => x.MembershipCandidates)
            .Where(x => departmentId == x.DepartmentId)
            .Where(x => x.GeneralElectionCommittees.Count != 0)
            .AsSplitQuery()
            .AsNoTracking()
            .ToListAsync();

        return committees;
    }

    public async Task<IEnumerable<Committee>> GetForGeneralElectionByOfficeId(Guid officeId)
    {
        var committees = await _dataContext.Committees
            .Include(x => x.GeneralElectionCommittees)
                .ThenInclude(x => x.MembershipCandidates)
            .Where(x => officeId == x.OfficeId)
            .Where(x => x.GeneralElectionCommittees.Count != 0)
            .AsSplitQuery()
            .AsNoTracking()
            .ToListAsync();

        return committees;
    }

    public async Task<IEnumerable<Committee>> GetByDescription(string description)
    {
        var filters = description.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var query = GetCommittees().AsNoTracking().Where(Committee.CanCreateMembershipExpression).Where(Committee.IsActiveExpression);

        foreach (var filter in filters)
        {
            var likeFilter = $"%{filter}%";

            query = query.Where(y =>
                EF.Functions.ILike(y.DescriptionGerman, likeFilter)
                || EF.Functions.ILike(y.DescriptionFrench, likeFilter)
                || EF.Functions.ILike(y.DescriptionItalian, likeFilter)
                || EF.Functions.ILike(y.DescriptionRomansh, likeFilter));
        }

        var committees = await query
            .OrderByDescending(y => y.BeginDate)
            .ToListAsync();

        return committees;
    }

    public void CreateForMigration(Committee committee)
    {
        _dataContext.Committees.Add(committee);
    }

    public async Task<PagedResult<Committee>> GetAll(PagingParameters paging, CommitteeFilterParameters? filter, string? sort, SortDirection? sortDirection)
    {
        var query = _dataContext.Committees
            .Include(item => item.ContactPoints)
            .Include(item => item.Memberships)
            .Include(item => item.CommitteeLevel)
            .Include(item => item.Department)
            .Include(item => item.Office)
            .Include(item => item.CommitteeType)
            .Include(item => item.TermOfOffice)
            .Include(item => item.Memberships)
                .ThenInclude(m => m.ElectionType)
            .Include(item => item.Memberships)
                .ThenInclude(m => m.Person)
                    .ThenInclude(p => p!.Interests)
            .FilterCommittees(filter)
            .AsSingleQuery();

        var count = await query
            .CountAsync();

        var items = await query
            .SortCommittees(sort ?? "description", sortDirection.GetValueOrDefault(SortDirection.Desc), _cultureService.GetCurrentUiCulture())
            .Skip(paging.PageIndex * paging.PageSize)
            .Take(paging.PageSize)
            .ToListAsync();

        return new PagedResult<Committee>
        {
            Index = paging.PageIndex,
            Total = count,
            Items = items
        };
    }

    public async Task<IEnumerable<Committee>> GetAllForExport(Guid departmentId, Guid officeId, Guid committeeId, CommitteeExportFilterParametersDto? filter)
    {
        var committees = await _dataContext.Committees
            .Where(x => x.BeginDate <= DateOnly.FromDateTime(DateTime.Now) && (x.EndDate == null || x.EndDate >= DateOnly.FromDateTime(DateTime.Now)))
            .Include(item => item.CommitteeLevel)
            .Include(item => item.Department)
            .Include(item => item.Office)
            .Include(item => item.CommitteeType)
            .Include(item => item.TermOfOffice)
            .FilterCommitteeByPermission(departmentId, officeId, committeeId, filter)
            .AsSingleQuery()
            .ToListAsync();

        return committees;
    }

    public async Task<IEnumerable<Committee>> GetAllForGeneralElection(Guid departmentId, Guid officeId, Guid committeeId)
    {
        return await _dataContext.Committees
            .Where(c => c.CommitteeLevelId == CommitteeLevel.FederalCouncilGuid && c.TermOfOfficeId == TermOfOffice.Period4YearsInGeneralElectionGuid)
            .Where(x => x.BeginDate <= DateOnly.FromDateTime(DateTime.Now) && (x.EndDate == null || x.EndDate >= DateOnly.FromDateTime(DateTime.Now)))
            .Include(item => item.Memberships)
            .ThenInclude(item => item.Person)
            .Include(item => item.Memberships)
            .ThenInclude(item => item.MembershipAddition)
            .Include(item => item.CommitteeType)
            .Include(item => item.Department)
            .FilterCommitteeByPermission(departmentId, officeId, committeeId)
            .AsSingleQuery()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Committee>> GetAllForGeneralElectionWithActiveMembers(Guid departmentId, Guid officeId, Guid committeeId)
    {
        return await _dataContext.Committees
            .Where(c => c.CommitteeLevelId == CommitteeLevel.FederalCouncilGuid && c.TermOfOfficeId == TermOfOffice.Period4YearsInGeneralElectionGuid)
            .Where(x => x.BeginDate <= DateOnly.FromDateTime(DateTime.Now) && (x.EndDate == null || x.EndDate >= DateOnly.FromDateTime(DateTime.Now)))
            .Include(item => item.CommitteeType)
            .Include(item => item.Department)
            .Include(item => item.Memberships).ThenInclude(item => item.Person).ThenInclude(item => item!.Gender)
            .Include(item => item.Memberships).ThenInclude(item => item.Person).ThenInclude(item => item!.Language)
            .FilterCommitteeByPermission(departmentId, officeId, committeeId)
            .Select(c => new Committee
            {
                Id = c.Id,
                Modified = c.Modified,
                ModifiedBy = c.ModifiedBy,
                Created = c.Created,
                CreatedBy = c.CreatedBy,
                TermOfOfficeDateId = c.TermOfOfficeDateId,
                DepartmentId = c.DepartmentId,
                Department = c.Department,
                CommitteeTypeId = c.CommitteeTypeId,
                CommitteeType = c.CommitteeType,
                IsDeleted = c.IsDeleted,
                DescriptionGerman = c.DescriptionGerman,
                DescriptionFrench = c.DescriptionFrench,
                DescriptionItalian = c.DescriptionItalian,
                DescriptionRomansh = c.DescriptionRomansh,
                JustificationMembers = c.JustificationMembers,
                JustificationGenders = c.JustificationGenders,
                JustificationLanguages = c.JustificationLanguages,
                MeasuresGenders = c.MeasuresGenders,
                MeasuresLanguages = c.MeasuresLanguages,
                RemarksBaseData = c.RemarksBaseData,
                RemarksBaseDataAdmin = c.RemarksBaseDataAdmin,
                // Filter only active memberships
                Memberships = c.Memberships
                    .Where(m => m.BeginDate <= DateOnly.FromDateTime(DateTime.Now) &&
                                m.EndDate > DateOnly.FromDateTime(DateTime.Now))
                    .ToList()
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<Committee>> GetByFilterForReport(ReportFilterParametersDto filterDto, Guid departmentId, Guid officeId, Guid committeeId)
    {
        var committees = await _dataContext.Committees
            .Include(item => item.CommitteeLevel)
            .Include(item => item.Department)
            .Include(item => item.Office)
            .Include(item => item.CommitteeType)
            .Include(item => item.TermOfOffice)
            .Include(item => item.Memberships)
            .ThenInclude(item => item.Person)
            .ThenInclude(item => item!.Gender)
            .Include(item => item.Memberships)
            .ThenInclude(item => item.Person)
            .ThenInclude(item => item!.Language)
            .Include(item => item.Memberships)
            .ThenInclude(item => item!.Function)
            .Include(item => item.Memberships)
            .ThenInclude(item => item!.ElectionType)
            .Include(item => item.MembershipAdditionsInGeneralElection)
            .FilterCommitteeByReportFilterParametersDto(filterDto, departmentId, officeId, committeeId)
            .AsSplitQuery()
            .Select(c => new Committee
            {
                Id = c.Id,
                Modified = c.Modified,
                ModifiedBy = c.ModifiedBy,
                Created = c.Created,
                CreatedBy = c.CreatedBy,
                BeginDate = c.BeginDate,
                EndDate = c.EndDate,
                TermOfOfficeDateId = c.TermOfOfficeDateId,
                DepartmentId = c.DepartmentId,
                Department = c.Department,
                OfficeId = c.OfficeId,
                Office = c.Office,
                CommitteeLevelId = c.CommitteeLevelId,
                CommitteeLevel = c.CommitteeLevel,
                CommitteeTypeId = c.CommitteeTypeId,
                CommitteeType = c.CommitteeType,
                IsDeleted = c.IsDeleted,
                DescriptionGerman = c.DescriptionGerman,
                DescriptionFrench = c.DescriptionFrench,
                DescriptionItalian = c.DescriptionItalian,
                DescriptionRomansh = c.DescriptionRomansh,
                JustificationMembers = c.JustificationMembers,
                JustificationGenders = c.JustificationGenders,
                JustificationLanguages = c.JustificationLanguages,
                MarketOrientated = c.MarketOrientated,
                MeasuresGenders = c.MeasuresGenders,
                MeasuresLanguages = c.MeasuresLanguages,
                RemarksBaseData = c.RemarksBaseData,
                RemarksBaseDataAdmin = c.RemarksBaseDataAdmin,
                VacanciesGeneralElection = c.VacanciesGeneralElection,
                LinkHomepageGerman = c.LinkHomepageGerman,
                LinkHomepageFrench = c.LinkHomepageFrench,
                LinkHomepageItalian = c.LinkHomepageItalian,
                LinkHomepageRomansh = c.LinkHomepageRomansh,
                MembershipAdditionsInGeneralElection = c.MembershipAdditionsInGeneralElection,
            })
            .OrderBy(c => c.DescriptionGerman)
            .ToListAsync();

        return committees;
    }

    public async Task<IEnumerable<Committee>> GetForOgdExport()
    {
        return await _dataContext.Committees
            .Where(x => x.BeginDate <= DateOnly.FromDateTime(DateTime.Now) && (x.EndDate == null || x.EndDate >= DateOnly.FromDateTime(DateTime.Now)))
            .Where(x => x.CommitteeTypeId != CommitteeType.CrossBorderFederalAgenciesCommitteeGuid)
            .Include(x => x.CommitteeType)
            .Include(x => x.Department)
            .Include(x => x.Memberships)
            .Include(x => x.ContactPoints)
            .Include(x => x.LegalForm)
            .Include(x => x.AppointmentDecisions)
                .ThenInclude(x => x.OriginalDocument)
            .AsSingleQuery()
            .ToListAsync();
    }

    public IEnumerable<Committee> GetAll()
    {
        return _dataContext.Committees
            .Include(item => item.Memberships)
            .Include(item => item.ContactPoints)
            .Include(item => item.AppointmentDecisions)
                .ThenInclude(item => item.OriginalDocument)
            .AsSingleQuery()
            .AsEnumerable();
    }

    public async Task<Committee> GetByIdForUpdate(Guid id, uint? updateDtoRowVersion = null)
    {
        var committee = await GetCommittees().FirstOrDefaultAsync(x => x.Id == id);

        if (committee is null)
        {
            throw new EntityNotFoundException($"Committee with ID {id} not found");
        }

        if (updateDtoRowVersion.HasValue && updateDtoRowVersion != committee.RowVersion)
        {
            _dataContext.Entry(committee).Property(x => x.RowVersion).OriginalValue = updateDtoRowVersion.Value;
        }

        return committee;
    }

    public async Task CommitChanges()
    {
        await _dataContext.SaveChangesAsync();
    }

    public async Task<Committee> Create(Committee committee)
    {
        var entry = await _dataContext.Committees.AddAsync(committee);
        await _dataContext.SaveChangesAsync();

        return entry.Entity;
    }

    public async Task<Committee[]> GetCommitteesForExport(DateOnly startDate, Guid departmentId, Guid officeId, Guid committeeId)
    {
        return await _dataContext.Committees
            .Where(x => !x.IsDeleted)
            .Where(x => !x.CommitteeType!.IsDeleted)
            .Where(x => x.BeginDate <= startDate && (x.EndDate == null || x.EndDate >= startDate))
            .Include(x => x.CommitteeType)
            .Include(x => x.Department)
            .Include(x => x.Office)
            .Include(x => x.CommitteeLevel)
            .Include(x => x.TermOfOffice)
            .Include(x => x.Memberships).ThenInclude(x => x.Person).ThenInclude(x => x!.Gender)
            .Include(x => x.Memberships).ThenInclude(x => x.Person).ThenInclude(x => x!.Interests)
            .Include(x => x.Memberships).ThenInclude(x => x.Person).ThenInclude(x => x!.Language)
            .Include(x => x.Memberships).ThenInclude(x => x.Person).ThenInclude(x => x!.Office)
            .Include(x => x.Memberships).ThenInclude(x => x.Person).ThenInclude(x => x!.CorrespondenceAddress).ThenInclude(x => x!.Canton)
            .Include(x => x.Memberships).ThenInclude(x => x.Function)
            .Include(x => x.Memberships).ThenInclude(x => x.ElectionType)
            .Include(x => x.Memberships).ThenInclude(x => x.ElectionOffice)
            .Include(x => x.Memberships).ThenInclude(x => x.MembershipAddition)
            .FilterCommitteeByPermission(departmentId, officeId, committeeId)
            .AsNoTracking()
            .AsSplitQuery()
            .ToArrayAsync();
    }

    public async Task<Committee[]> GetCommitteesWithInterestsForExport(DateOnly startDate, Guid departmentId, Guid officeId, Guid committeeId)
    {
        return await _dataContext.Committees
            .Where(x => !x.IsDeleted)
            .Where(x => !x.CommitteeType!.IsDeleted)
            .Where(x => x.BeginDate <= startDate && (x.EndDate == null || x.EndDate >= startDate))
            .Include(x => x.CommitteeType)
            .Include(x => x.Department)
            .Include(x => x.Office)
            .Include(x => x.CommitteeLevel)
            .Include(x => x.TermOfOffice)
            .Include(x => x.Memberships).ThenInclude(x => x.Person).ThenInclude(x => x!.Interests).ThenInclude(x => x.InterestCommittee)
            .Include(x => x.Memberships).ThenInclude(x => x.Person).ThenInclude(x => x!.Interests).ThenInclude(x => x.InterestFunction)
            .Include(x => x.Memberships).ThenInclude(x => x.Person).ThenInclude(x => x!.Interests).ThenInclude(x => x.InterestLegalForm)
            .Include(x => x.Memberships).ThenInclude(x => x.Person).ThenInclude(x => x!.Interests).ThenInclude(x => x.LegalForm)
            .FilterCommitteeByPermission(departmentId, officeId, committeeId)
            .AsNoTracking()
            .AsSplitQuery()
            .ToArrayAsync();
    }

    public async Task<Committee[]> GetCommitteesWithContactPointsForExport(DateOnly startDate, Guid departmentId, Guid officeId, Guid committeeId)
    {
        return await _dataContext.Committees
            .Where(x => !x.IsDeleted)
            .Where(x => !x.CommitteeType!.IsDeleted)
            .Where(x => x.BeginDate <= startDate && (x.EndDate == null || x.EndDate >= startDate))
            .Include(x => x.CommitteeType)
            .Include(x => x.Department)
            .Include(x => x.Office)
            .Include(x => x.CommitteeLevel)
            .Include(x => x.TermOfOffice)
            .Include(x => x.ContactPoints)
            .FilterCommitteeByPermission(departmentId, officeId, committeeId)
            .AsNoTracking()
            .AsSplitQuery()
            .ToArrayAsync();
    }

    public async Task<Committee[]> GetCommitteesForRegionExport(DateOnly startDate, Guid departmentId, Guid officeId, Guid committeeId)
    {
        return await _dataContext.Committees
            .Where(x => !x.IsDeleted)
            .Where(x => !x.CommitteeType!.IsDeleted)
            .Where(x => x.BeginDate <= startDate && (x.EndDate == null || x.EndDate >= startDate))
            .Include(x => x.Memberships)
            .ThenInclude(x => x.Person)
            .ThenInclude(x => x!.CorrespondenceAddress)
            .ThenInclude(x => x!.Canton)
            .FilterCommitteeByPermission(departmentId, officeId, committeeId)
            .AsNoTracking()
            .AsSplitQuery()
            .ToArrayAsync();
    }

    public async Task<Committee[]> GetCommitteeDataForStatistics()
    {
        // For statistic, not all 5 active committeeTypes are relevant, we filter here for the valid ones!
        return await _dataContext.Committees
            .Where(x => !x.IsDeleted)
            .Where(x => !x.CommitteeType!.IsDeleted)
            .Where(x => x.CommitteeTypeId == CommitteeType.FederalAgenciesCommitteeGuid || x.CommitteeTypeId == CommitteeType.AuthoritiesCommissionGuid ||
                x.CommitteeTypeId == CommitteeType.ManagementCommitteeGuid || x.CommitteeTypeId == CommitteeType.AdministrationCommissionGuid)
            .Where(x => x.BeginDate <= DateOnly.FromDateTime(DateTime.Now) && (x.EndDate == null || x.EndDate >= DateOnly.FromDateTime(DateTime.Now)))
            .Include(x => x.Department)
            .Include(x => x.CommitteeType)
            .Include(x => x.Memberships)
            .ThenInclude(x => x.Person)
            .AsNoTracking()
            .AsSplitQuery()
            .ToArrayAsync();
    }

    private IQueryable<Committee> GetCommittees()
    {
        return _dataContext.Committees
            .Include(item => item.Memberships).ThenInclude(m => m.ElectionType)
            .Include(item => item.CommitteeLevel)
            .Include(item => item.Department)
            .Include(item => item.Office)
            .Include(item => item.CommitteeType)
            .Include(item => item.TermOfOffice)
            .Include(item => item.LegalForm)
            .Include(item => item.TermOfOfficeDate)
            .Include(item => item.Memberships)
            .Include(item => item.Memberships).ThenInclude(m => m.Person).ThenInclude(p => p!.Gender)
            .Include(item => item.Memberships).ThenInclude(m => m.Person).ThenInclude(p => p!.Language)
            .Include(item => item.Memberships).ThenInclude(m => m.Person).ThenInclude(p => p!.Interests)
            .Include(x => x.ContactPoints).ThenInclude(x => x.ContactPointType)
            .Include(x => x.ContactPoints).ThenInclude(x => x.Language)
            .Include(x => x.ContactPoints).ThenInclude(x => x.Gender)
            .Include(x => x.MembershipAdditionsInGeneralElection)
            .AsSplitQuery();
    }
}
