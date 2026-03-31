using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting;
using Bk.APG.CrossCutting.Exception;
using Bk.APG.Infrastructure.DataSource.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Infrastructure.DataSource.Repositories;

public class GeneralElectionCommitteeRepository : IGeneralElectionCommitteeRepository
{
    private readonly DataContext _dataContext;
    private readonly ICultureService _cultureService;

    public GeneralElectionCommitteeRepository(DataContext dataContext, ICultureService cultureService)
    {
        _dataContext = dataContext;
        _cultureService = cultureService;
    }

    public async Task<GeneralElectionCommittee> GetById(Guid id)
    {
        var generalElectionCommittee = await GetGeneralElectionCommittees().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        if (generalElectionCommittee is null)
        {
            throw new EntityNotFoundException($"GeneralElectionCommittee Id={id} not found");
        }

        return generalElectionCommittee;
    }

    public async Task<GeneralElectionCommittee> GetByCommitteeId(Guid committeeId)
    {
        var generalElectionCommittee = await GetGeneralElectionCommittees().AsNoTracking().FirstOrDefaultAsync(x => x.CommitteeId == committeeId);

        return generalElectionCommittee ?? throw new EntityNotFoundException($"GeneralElectionCommittee with CommitteeId={committeeId} not found");
    }

    public async Task<GeneralElectionCommittee> GetForCandidateListExport(Guid committeeId, IEnumerable<Guid> membershipCandidateIds)
    {
        var generalElectionCommittee = await GetGeneralElectionCommittees()
            .Include(y => y.MembershipCandidates
                .Where(mc => !membershipCandidateIds.Any() || membershipCandidateIds.Contains(mc.Id)))
                    .ThenInclude(mc => mc.Person)
                    .ThenInclude(p => p!.Interests)
            .Include(y => y.MembershipCandidates
                .Where(mc => !membershipCandidateIds.Any() || membershipCandidateIds.Contains(mc.Id)))
                    .ThenInclude(mc => mc.Person)
                    .ThenInclude(p => p!.CorrespondenceAddress)
            .Include(y => y.MembershipCandidates
                .Where(mc => !membershipCandidateIds.Any() || membershipCandidateIds.Contains(mc.Id)))
                    .ThenInclude(mc => mc.Person)
                    .ThenInclude(p => p!.Occupations)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CommitteeId == committeeId);

        return generalElectionCommittee ?? throw new EntityNotFoundException($"GeneralElectionCommittee with CommitteeId={committeeId} not found");
    }

    public async Task<IEnumerable<GeneralElectionCommittee>> GetByDepartmentId(Guid departmentId)
    {
        var generalElectionCommittees = await _dataContext.GeneralElectionCommittees
            .Include(x => x.Department)
            .Where(x => departmentId == x.DepartmentId)
            .ToListAsync();

        return generalElectionCommittees;
    }

    public async Task<IEnumerable<GeneralElectionCommittee>> GetByOfficeId(Guid officeId)
    {
        return await _dataContext.GeneralElectionCommittees
            .Where(x => officeId == x.OfficeId)
            .ToListAsync();
    }

    public async Task<PagedResult<GeneralElectionCommittee>> GetAll(PagingParameters paging, GeneralElectionCommitteeFilterParameters filter, string? sort, SortDirection? sortDirection)
    {
        var query = _dataContext.GeneralElectionCommittees
            .Include(item => item.CommitteeLevel)
            .Include(item => item.Department)
            .Include(item => item.Office)
            .Include(item => item.CommitteeType)
            .Include(item => item.TermOfOffice)
            .Include(item => item.MembershipCandidates)
            .Include(item => item.Committee)
                .ThenInclude(item => item!.TermOfOfficeDate)
            .FilterGeneralElectionCommittees(filter)
            .AsSingleQuery();

        var count = await query
            .CountAsync();

        var items = await query
            .SortGeneralElectionCommittees(sort ?? "description", sortDirection.GetValueOrDefault(SortDirection.Desc), _cultureService.GetCurrentUiCulture())
            .Skip(paging.PageIndex * paging.PageSize)
            .Take(paging.PageSize)
            .ToListAsync();

        return new PagedResult<GeneralElectionCommittee>
        {
            Index = paging.PageIndex,
            Total = count,
            Items = items
        };
    }

    public async Task<IEnumerable<GeneralElectionCommittee>> GetByFilterForReport(ReportFilterParametersDto filterDto, Guid departmentId, Guid officeId, Guid committeeId)
    {
        var committees = await _dataContext.GeneralElectionCommittees
            .Where(x => !filterDto.ReleasedCommittees || x.ReleaseGeneralElection == true)
            .Include(item => item.CommitteeLevel)
            .Include(item => item.Department)
            .Include(item => item.Office)
            .Include(item => item.CommitteeType)
            .Include(item => item.TermOfOffice)
            .Include(item => item.MembershipCandidates)
            .ThenInclude(item => item.Person)
            .ThenInclude(item => item!.Gender)
            .Include(item => item.MembershipCandidates)
            .ThenInclude(item => item.Person)
            .ThenInclude(item => item!.Language)
            .Include(item => item.MembershipCandidates)
            .ThenInclude(item => item!.Function)
            .Include(item => item.MembershipCandidates)
            .ThenInclude(item => item!.ElectionType)
            .FilterGeneralElectionCommitteeByPermission(departmentId, officeId, committeeId)
            .Where(c => !filterDto.CommitteesWithActiveMembership || c.MembershipCandidates.Count > 0)
            .AsSplitQuery()
            .Select(c => new GeneralElectionCommittee
            {
                Id = c.Id,
                Modified = c.Modified,
                ModifiedBy = c.ModifiedBy,
                Created = c.Created,
                CreatedBy = c.CreatedBy,
                BeginDate = c.BeginDate,
                EndDate = c.EndDate,
                TermOfOfficeDateId = c.TermOfOfficeDateId,
                CommitteeId = c.CommitteeId,
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
                IsValidated = c.IsValidated,
                WasValidatedOnce = c.WasValidatedOnce,
                IsFederalCouncilProposalDirty = c.IsFederalCouncilProposalDirty,
                VacanciesGeneralElection = c.VacanciesGeneralElection,
                SelectionProcedure = c.SelectionProcedure,
                CandidateListStateId = c.CandidateListStateId,
                AssignedToRole = c.AssignedToRole,
                MembershipCandidates = c.MembershipCandidates.ToList()
            })
            .OrderBy(c => c.DescriptionGerman)
            .ToListAsync();

        return committees;
    }

    public async Task<IEnumerable<GeneralElectionCommittee>> GetAllForFormLetterPreview(GeneralElectionCommitteeExportFilterParameters filterDto, List<Guid> electionTypesIds)
    {
        var committees = await _dataContext.GeneralElectionCommittees
            .Include(item => item.CommitteeLevel)
            .Include(item => item.Department)
            .Include(item => item.Office)
            .Include(item => item.CommitteeType)
            .Include(item => item.TermOfOffice)
            .Include(item => item.MembershipCandidates)
            .ThenInclude(item => item.Person)
            .ThenInclude(item => item!.Gender)
            .Include(item => item.MembershipCandidates)
            .ThenInclude(item => item.Person)
            .ThenInclude(item => item!.Language)
            .Include(item => item.MembershipCandidates)
            .ThenInclude(item => item!.Function)
            .Include(item => item.MembershipCandidates)
            .ThenInclude(item => item!.ElectionType)
            .Include(item => item.Committee)
            .FilterGeneralElectionCommitteesForExport(filterDto)
        .AsSplitQuery()
            .Select(c => new GeneralElectionCommittee
            {
                Id = c.Id,
                Modified = c.Modified,
                ModifiedBy = c.ModifiedBy,
                Created = c.Created,
                CreatedBy = c.CreatedBy,
                BeginDate = c.BeginDate,
                EndDate = c.EndDate,
                TermOfOfficeDateId = c.TermOfOfficeDateId,
                CommitteeId = c.CommitteeId,
                DepartmentId = c.DepartmentId,
                Department = c.Department,
                CommitteeTypeId = c.CommitteeTypeId,
                CommitteeType = c.CommitteeType,
                OfficeId = c.OfficeId,
                Office = c.Office,
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
                IsValidated = c.IsValidated,
                WasValidatedOnce = c.WasValidatedOnce,
                IsFederalCouncilProposalDirty = c.IsFederalCouncilProposalDirty,
                VacanciesGeneralElection = c.VacanciesGeneralElection,
                SelectionProcedure = c.SelectionProcedure,
                CandidateListStateId = c.CandidateListStateId,
                AssignedToRole = c.AssignedToRole,
                MembershipCandidates = c.MembershipCandidates
                    .Where(m => m.Person != null &&
                        (filterDto.CorrespondenceLanguageIds == null || !filterDto.CorrespondenceLanguageIds.Any() ||
                         filterDto.CorrespondenceLanguageIds!.Contains(
                             m.Person.CorrespondenceLanguageId)) &&
                        (electionTypesIds == null || electionTypesIds.Count == 0 ||
                         electionTypesIds!.Contains(
                             m.ElectionTypeId)))
                    .ToList()
            })
            .ToListAsync();

        return committees;
    }

    public async Task<IEnumerable<GeneralElectionCommittee>> GetAllForFormLetter(FormLetterFilterParameters filterDto, List<Guid> electionTypeIds)
    {
        var committees = await _dataContext.GeneralElectionCommittees
            .Include(item => item.CommitteeLevel)
            .Include(item => item.Department)
            .Include(item => item.Office)
            .Include(item => item.CommitteeType)
            .Include(item => item.TermOfOffice)
            .Include(item => item.MembershipCandidates)
            .ThenInclude(item => item.Person)
            .ThenInclude(item => item!.Gender)
            .Include(item => item.MembershipCandidates)
            .ThenInclude(item => item.Person)
            .ThenInclude(item => item!.Language)
            .Include(item => item.MembershipCandidates)
            .ThenInclude(item => item.Person)
            .ThenInclude(item => item!.CorrespondenceAddress)
            .ThenInclude(item => item!.Country)
            .Include(item => item.MembershipCandidates)
            .ThenInclude(item => item.Person)
            .ThenInclude(item => item!.Salutation)
            .Include(item => item.MembershipCandidates)
            .ThenInclude(item => item!.Function)
            .Include(item => item.MembershipCandidates)
            .ThenInclude(item => item!.ElectionType)
            .Include(item => item.Committee)
            .FilterGeneralElectionCommitteesForFormLetter(filterDto, electionTypeIds)
        .AsSplitQuery()
            .Select(c => new GeneralElectionCommittee
            {
                Id = c.Id,
                Modified = c.Modified,
                ModifiedBy = c.ModifiedBy,
                Created = c.Created,
                CreatedBy = c.CreatedBy,
                BeginDate = c.BeginDate,
                EndDate = c.EndDate,
                TermOfOfficeDateId = c.TermOfOfficeDateId,
                CommitteeId = c.CommitteeId,
                DepartmentId = c.DepartmentId,
                Department = c.Department,
                CommitteeTypeId = c.CommitteeTypeId,
                CommitteeType = c.CommitteeType,
                OfficeId = c.OfficeId,
                Office = c.Office,
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
                IsValidated = c.IsValidated,
                WasValidatedOnce = c.WasValidatedOnce,
                IsFederalCouncilProposalDirty = c.IsFederalCouncilProposalDirty,
                VacanciesGeneralElection = c.VacanciesGeneralElection,
                SelectionProcedure = c.SelectionProcedure,
                CandidateListStateId = c.CandidateListStateId,
                AssignedToRole = c.AssignedToRole,
                // bring only candidates, which match by language and electiontype
                MembershipCandidates = c.MembershipCandidates
                    .Where(m => m.Person != null &&
                        (filterDto.CorrespondenceLanguageIds == null || !filterDto.CorrespondenceLanguageIds.Any() ||
                         filterDto.CorrespondenceLanguageIds!.Contains(
                             m.Person.CorrespondenceLanguageId)) &&
                        (electionTypeIds == null || electionTypeIds.Count == 0 ||
                         electionTypeIds!.Contains(
                             m.ElectionTypeId)))
                    .ToList()
            })
            .ToListAsync();

        return committees;
    }

    public async Task<IEnumerable<GeneralElectionCommittee>> GetAll()
    {
        return await _dataContext.GeneralElectionCommittees
            .Include(item => item.CommitteeLevel)
            .Include(item => item.Department)
            .Include(item => item.Office)
            .Include(item => item.CommitteeType)
            .Include(item => item.TermOfOffice)
            .Include(item => item.MembershipCandidates)
            .ThenInclude(item => item.Person)
            .ToListAsync();
    }

    public async Task<GeneralElectionCommittee> GetByIdForUpdate(Guid id, uint? updateDtoRowVersion = null)
    {
        var generalElectionCommittee = await GetGeneralElectionCommittees().FirstOrDefaultAsync(x => x.Id == id);

        if (generalElectionCommittee is null)
        {
            throw new EntityNotFoundException($"GeneralElectionCommittee with ID {id} not found");
        }

        if (updateDtoRowVersion.HasValue && updateDtoRowVersion != generalElectionCommittee.RowVersion)
        {
            _dataContext.Entry(generalElectionCommittee).Property(x => x.RowVersion).OriginalValue = updateDtoRowVersion.Value;
        }

        return generalElectionCommittee;
    }

    public async Task CommitChanges()
    {
        await _dataContext.SaveChangesAsync();
    }

    public async Task<GeneralElectionCommittee> Create(GeneralElectionCommittee generalElectionCommittee)
    {
        var entry = await _dataContext.GeneralElectionCommittees.AddAsync(generalElectionCommittee);
        await _dataContext.SaveChangesAsync();

        return entry.Entity;
    }

    public async Task DeleteAll()
    {
        var allEntities = _dataContext.GeneralElectionCommittees.ToList();
        _dataContext.GeneralElectionCommittees.RemoveRange(allEntities);
        await _dataContext.SaveChangesAsync();
    }

    public async Task<GeneralElectionCommittee> GetByCommitteeIdForUpdate(Guid committeeId, uint? updateDtoRowVersion = null)
    {
        var generalElectionCommittee = await GetGeneralElectionCommittees().FirstOrDefaultAsync(x => x.CommitteeId == committeeId);

        if (generalElectionCommittee is null)
        {
            throw new EntityNotFoundException($"GeneralElectionCommittee with CommitteeID {committeeId} not found");
        }

        if (updateDtoRowVersion.HasValue && updateDtoRowVersion != generalElectionCommittee.RowVersion)
        {
            _dataContext.Entry(generalElectionCommittee).Property(x => x.RowVersion).OriginalValue = updateDtoRowVersion.Value;
        }

        return generalElectionCommittee;
    }

    private IQueryable<GeneralElectionCommittee> GetGeneralElectionCommittees()
    {
        return _dataContext.GeneralElectionCommittees
            .Include(item => item.MembershipCandidates)
                .ThenInclude(c => c.Gender)
            .Include(item => item.MembershipCandidates)
                .ThenInclude(c => c.Language)
            .Include(item => item.MembershipCandidates)
                .ThenInclude(c => c.Function)
            .Include(item => item.MembershipCandidates)
                .ThenInclude(c => c.MembershipAddition)
            .Include(item => item.MembershipCandidates)
                .ThenInclude(c => c.ElectionType)
            .Include(item => item.MembershipCandidates)
                .ThenInclude(c => c.Person)
                    .ThenInclude(p => p!.Gender)
            .Include(item => item.MembershipCandidates)
                .ThenInclude(c => c.Person)
                    .ThenInclude(p => p!.Language)
            .Include(item => item.MembershipCandidates)
                .ThenInclude(c => c.Person)
                    .ThenInclude(p => p!.LegislaturePeriods)
            .Include(item => item.MembershipCandidates)
                .ThenInclude(c => c.Person!.Memberships)
                    .ThenInclude(m => m.Committee)
            .Include(item => item.MembershipCandidates)
                .ThenInclude(c => c.Person!.Memberships)
                    .ThenInclude(m => m.Function)
            .Include(item => item.MembershipCandidates)
                .ThenInclude(c => c.Person!.Interests)
                    .ThenInclude(i => i.InterestCommittee)
            .Include(item => item.MembershipCandidates)
                .ThenInclude(c => c.Person!.Interests)
                    .ThenInclude(i => i.InterestFunction)
            .Include(item => item.MembershipCandidates)
                .ThenInclude(c => c.Person!.Interests)
                    .ThenInclude(i => i.InterestLegalForm)
            .Include(item => item.MembershipCandidates)
                .ThenInclude(c => c.Person!.Occupations)
            .Include(x => x.Department!.GeneralGenderMeasure)
            .Include(x => x.Department!.GeneralLanguageMeasure)
            .Include(item => item.Committee!.ContactPoints)
            .Include(item => item.CommitteeLevel)
            .Include(item => item.Department)
            .Include(item => item.Office)
            .Include(item => item.CommitteeType)
            .Include(item => item.TermOfOffice)
            .Include(item => item.LegalForm)
            .Include(item => item.TermOfOfficeDate)
            .Include(item => item.CandidateListState)
            .AsSplitQuery();
    }
}
