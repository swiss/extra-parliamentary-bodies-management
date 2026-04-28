using Bk.APG.Business.Extensions;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting;
using Bk.APG.CrossCutting.Exception;
using Bk.APG.Infrastructure.DataSource.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Infrastructure.DataSource.Repositories;

public class WorklistTaskRepository : IWorklistTaskRepository
{
    private readonly DataContext _dataContext;
    private readonly ICultureService _cultureService;
    private readonly IAuthorizationService _authorizationService;

    public WorklistTaskRepository(DataContext dataContext, ICultureService cultureService, IAuthorizationService authorizationService)
    {
        _dataContext = dataContext;
        _cultureService = cultureService;
        _authorizationService = authorizationService;
    }

    public async Task<PagedResult<WorklistTask>> GetAll(PagingParameters paging, WorklistFilterParameters? filter, string? sort, SortDirection? sortDirection)
    {
        ArgumentNullException.ThrowIfNull(paging);

        var isAdmin = _authorizationService.IsAdmin;
        var eiamAssignmentIds = isAdmin ? [] : (await _authorizationService.GetCurrentEiamAssignment()).GetSearchableIds().ToList();
        var query = _dataContext.WorklistTasks
            .Where(w => !w.IsDeleted)
            .Include(item => item.WorklistTaskType)
            .Include(item => item.WorklistTaskState)
            .Include(item => item.Department)
            .Include(item => item.Office)
            .Include(item => item.Committee)
            .Include(item => item.Person)
            .Include(item => item.AssignedTo!.Department!.Offices)
            .Include(item => item.AssignedTo!.Office)
            .Include(item => item.AssignedTo!.Committee)
            .Include(item => item.AssignedBy!.Department!.Offices)
            .Include(item => item.AssignedBy!.Office)
            .Include(item => item.AssignedBy!.Committee)
            .FilterWorklistTask(filter, isAdmin, eiamAssignmentIds)
            .AsSplitQuery()
            .AsNoTracking();

        var count = await query
            .CountAsync();

        var items = await query
            .SortWorklistTasks(sort ?? "dueDate", sortDirection.GetValueOrDefault(SortDirection.Asc), _cultureService.GetCurrentUiCulture())
            .Skip(paging.PageIndex * paging.PageSize)
            .Take(paging.PageSize)
            .ToListAsync();

        return new PagedResult<WorklistTask>
        {
            Index = paging.PageIndex,
            Total = count,
            Items = items
        };
    }

    public async Task<WorklistTask> GetByIdForUpdate(Guid id, uint? updateDtoRowVersion = null)
    {
        var task = await _dataContext.WorklistTasks
            .Where(w => !w.IsDeleted)
            .Include(x => x.WorklistTaskType)
            .Include(x => x.WorklistTaskState)
            .Include(x => x.Department)
            .Include(item => item.AssignedTo!.Department!.Offices)
            .Include(item => item.AssignedTo!.Office)
            .Include(item => item.AssignedTo!.Committee)
            .Include(item => item.AssignedBy!.Department!.Offices)
            .Include(item => item.AssignedBy!.Office)
            .Include(item => item.AssignedBy!.Committee)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (task == null)
        {
            throw new EntityNotFoundException($"WorklistTask with ID {id} not found");
        }

        if (updateDtoRowVersion.HasValue && updateDtoRowVersion != task.RowVersion)
        {
            _dataContext.Entry(task).Property(x => x.RowVersion).OriginalValue = updateDtoRowVersion.Value;
        }

        return task;
    }

    public async Task<WorklistTask> GetByIdForForward(Guid id)
    {
        var task = await _dataContext.WorklistTasks
            .Where(w => !w.IsDeleted)
            .Include(w => w.Department)
            .Include(w => w.Office)
            .Include(w => w.Committee)
            .Include(w => w.AssignedTo!.Children)
            .Include(w => w.AssignedBy)
            .Include(w => w.ParentTask)
            .FirstOrDefaultAsync(w => w.Id == id);

        return task ?? throw new EntityNotFoundException($"WorklistTask with ID {id} not found");
    }

    public async Task<IEnumerable<WorklistTask>> GetByTermOfOfficeDateId(Guid id)
    {
        var worklistTasks = await _dataContext.WorklistTasks
            .Where(w => w.TermOfOfficeDateId == id && !w.IsDeleted)
            .ToListAsync();

        return worklistTasks;
    }

    public async Task<IEnumerable<WorklistTask>> GetByPersonOrMemberships(Guid personId, IEnumerable<Guid> membershipIds, IEnumerable<Guid> membershipCandidateIds)
    {
        var membershipIdSet = membershipIds.ToHashSet();
        var membershipCandidateIdSet = membershipCandidateIds.ToHashSet();

        return await _dataContext.WorklistTasks
            .Where(w =>
                w.PersonId == personId ||
                (w.MembershipId.HasValue && membershipIdSet.Contains(w.MembershipId.Value)) ||
                (w.MembershipCandidateId.HasValue && membershipCandidateIdSet.Contains(w.MembershipCandidateId.Value)))
            .Where(w => !w.IsDeleted)
            .ToListAsync();
    }

    public async Task<WorklistTask> Create(WorklistTask worklistTask)
    {
        var entry = await _dataContext.WorklistTasks.AddAsync(worklistTask);
        await _dataContext.SaveChangesAsync();

        return entry.Entity;
    }

    public async Task CreateRange(IEnumerable<WorklistTask> worklistTasks)
    {
        ArgumentNullException.ThrowIfNull(worklistTasks);

        foreach (var worklistTask in worklistTasks)
        {
            await _dataContext.WorklistTasks.AddAsync(worklistTask);
        }

        await _dataContext.SaveChangesAsync();
    }

    public void DeleteRange(IEnumerable<WorklistTask> worklistTasks)
    {
        _dataContext.WorklistTasks.RemoveRange(worklistTasks);
    }

    public async Task Update(WorklistTask worklistTask)
    {
        _dataContext.WorklistTasks.Update(worklistTask);
        await _dataContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<WorklistTask>> GetAllByGeneralElectionCommitteeId(Guid generalElectionCommitteeId)
    {
        return await _dataContext.WorklistTasks.Include(x => x.AssignedTo)
            .ThenInclude(y => y!.Parent)
            .Where(x => x.GeneralElectionCommitteeId == generalElectionCommitteeId && !x.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<WorklistTask>> GetAllByCommitteeId(Guid committeeId)
    {
        return await _dataContext.WorklistTasks.Include(x => x.AssignedTo)
            .ThenInclude(y => y!.Parent)
            .Where(x => x.CommitteeId == committeeId && !x.IsDeleted)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task CommitChanges()
    {
        await _dataContext.SaveChangesAsync();
    }

    public async Task<List<WorklistTask>> GetByWorklistTaskTypeId(Guid worklistTaskTypeId)
    {
        return await _dataContext.WorklistTasks
            .Include(w => w.ParentTask)
            .Include(w => w.AssignedBy)
            .Where(x => x.WorklistTaskTypeId == worklistTaskTypeId && !x.IsDeleted)
            .ToListAsync();
    }

    public async Task<List<WorklistTask>> GetAllByPersonId(Guid personId)
    {
        return await _dataContext.WorklistTasks
            .Where(x => x.PersonId == personId && !x.IsDeleted)
            .ToListAsync();
    }

    public async Task<List<WorklistTask>> GetByDepartmentIdsAndWorklistTaskTypeIds(IEnumerable<Guid> departmentIds, IEnumerable<Guid> worklistTaskTypeIds)
    {
        return await _dataContext.WorklistTasks
            .Where(x => x.DepartmentId.HasValue &&
                departmentIds.Contains(x.DepartmentId.Value) &&
                worklistTaskTypeIds.Contains(x.WorklistTaskTypeId))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<WorklistTask>> GetByDepartmentIdAndWorklistTaskTypeIdsForUpdate(Guid departmentId, IEnumerable<Guid> worklistTaskTypeIds)
    {
        return await _dataContext.WorklistTasks
            .Where(x => x.DepartmentId == departmentId && worklistTaskTypeIds.Contains(x.WorklistTaskTypeId) && !x.IsDeleted)
            .ToListAsync();
    }

    public async Task SetAllWorklistTasksToIsDeleted()
    {
        await _dataContext.WorklistTasks
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(w => w.IsDeleted, true));
    }
}
